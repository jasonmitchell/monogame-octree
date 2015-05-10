using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace OctreeDemo
{
    public class OctreeNode
    {
        private readonly List<OctreeNode> childNodes = new List<OctreeNode>(8);
        private readonly List<BoundingSphere> contents = new List<BoundingSphere>();

        private readonly BoundingBox boundingBox;
        private readonly Vector3 position;

        private int propagationThreshold;
        private int maximumDepth;
        private int nodeDepth;

        private bool isPartitioned;

        public static OctreeNode CreateTree(int itemsInNode, int maxDepth, Vector3 rootPosition, Vector3 worldDimensions)
        {
            OctreeNode node = new OctreeNode(null, rootPosition, worldDimensions);
            node.maximumDepth = maxDepth;
            node.propagationThreshold = itemsInNode;
            node.nodeDepth = 1;

            return node;
        }

        private OctreeNode(OctreeNode parent, Vector3 nodePosition, Vector3 dimensions)
        {
            if (parent != null)
            {
                propagationThreshold = parent.propagationThreshold;
                maximumDepth = parent.maximumDepth;
                nodeDepth = parent.nodeDepth + 1;
            }

            position = nodePosition;
            boundingBox = new BoundingBox(nodePosition, nodePosition + dimensions);

            DebugShapeRenderer.AddBoundingBox(boundingBox, Color.Red, 10000000000);
        }

        public void Insert(BoundingSphere item)
        {
            DebugShapeRenderer.AddBoundingSphere(item, Color.LimeGreen, 100000000000);

            if (TryInsertInChildNode(item))
                return;

            contents.Add(item);

            if (!isPartitioned && (maximumDepth < 0 || nodeDepth < maximumDepth) && contents.Count >= propagationThreshold)
                PartitionNode();
        }

        private bool TryInsertInChildNode(BoundingSphere item)
        {
            if (isPartitioned)
            {
                foreach (OctreeNode node in childNodes)
                {
                    if(node.boundingBox.Contains(item) == ContainmentType.Contains)
                    {
                        node.Insert(item);
                        return true;
                    }
                }
            }

            return false;
        }

        public void Remove(BoundingSphere item)
        {
            if (contents.Contains(item))
                contents.Remove(item);
            else
            {
                foreach(OctreeNode node in childNodes)
                    node.Remove(item);
            }
        }

        private void RemoveAt(int index)
        {
            if (index < contents.Count)
                contents.RemoveAt(index);
        }

        private void PartitionNode()
        {
            Vector3 dimensions = (boundingBox.Max - boundingBox.Min) / 2;

            childNodes.Add(new OctreeNode(this, position, dimensions));
            childNodes.Add(new OctreeNode(this, position + new Vector3(dimensions.X, 0, 0), dimensions));
            childNodes.Add(new OctreeNode(this, position + new Vector3(0, 0, dimensions.Z), dimensions));
            childNodes.Add(new OctreeNode(this, position + new Vector3(dimensions.X, 0, dimensions.Z), dimensions));

            childNodes.Add(new OctreeNode(this, position + new Vector3(0, dimensions.Y, 0), dimensions));
            childNodes.Add(new OctreeNode(this, position + new Vector3(dimensions.X, dimensions.Y, 0), dimensions));
            childNodes.Add(new OctreeNode(this, position + new Vector3(0, dimensions.Y, dimensions.Z), dimensions));
            childNodes.Add(new OctreeNode(this, position + new Vector3(dimensions.X, dimensions.Y, dimensions.Z), dimensions));

            isPartitioned = true;

            int i = 0;
            while (i < contents.Count)
            {
                if (!TryPushItemDown(i))
                    i++;
            }
        }

        private bool TryPushItemDown(int index)
        {
            if(TryInsertInChildNode(contents[index]))
            {
                RemoveAt(index);
                return true;
            }

            return false;
        }

        public List<BoundingSphere> Intersects(Ray ray)
        {
            List<BoundingSphere> objects = new List<BoundingSphere>();

            if (boundingBox.Intersects(ray) != null)
            {
                objects.AddRange(contents.Where(item => item.Intersects(ray) != null));

                foreach (OctreeNode node in childNodes)
                    objects.AddRange(node.Intersects(ray));
            }

            return objects;
        }

        public List<BoundingSphere> TreeContents
        {
            get
            {
                List<BoundingSphere> results = new List<BoundingSphere>(contents);

                foreach(OctreeNode node in childNodes)
                    results.AddRange(node.contents);

                return results;
            }
        }
    }
}
