using System;

namespace MIConvexHull
{
	internal class ObjectManager
	{
		private readonly int Dimension;

		private FaceConnector ConnectorStack;

		private readonly SimpleList<DeferredFace> DeferredFaceStack;

		private readonly SimpleList<IndexBuffer> EmptyBufferStack;

		private ConvexFaceInternal[] FacePool;

		private int FacePoolSize;

		private int FacePoolCapacity;

		private readonly IndexBuffer FreeFaceIndices;

		private readonly ConvexHullAlgorithm Hull;

		public ObjectManager(ConvexHullAlgorithm hull)
		{
			Dimension = hull.NumOfDimensions;
			Hull = hull;
			FacePool = hull.FacePool;
			FacePoolSize = 0;
			FacePoolCapacity = hull.FacePool.Length;
			FreeFaceIndices = new IndexBuffer();
			EmptyBufferStack = new SimpleList<IndexBuffer>();
			DeferredFaceStack = new SimpleList<DeferredFace>();
		}

		public void DepositFace(int faceIndex)
		{
			int[] adjacentFaces = FacePool[faceIndex].AdjacentFaces;
			for (int i = 0; i < adjacentFaces.Length; i++)
			{
				adjacentFaces[i] = -1;
			}
			FreeFaceIndices.Push(faceIndex);
		}

		private void ReallocateFacePool()
		{
			ConvexFaceInternal[] array = new ConvexFaceInternal[2 * FacePoolCapacity];
			bool[] array2 = new bool[2 * FacePoolCapacity];
			Array.Copy(FacePool, array, FacePoolCapacity);
			Buffer.BlockCopy(Hull.AffectedFaceFlags, 0, array2, 0, FacePoolCapacity);
			FacePoolCapacity = 2 * FacePoolCapacity;
			Hull.FacePool = array;
			FacePool = array;
			Hull.AffectedFaceFlags = array2;
		}

		private int CreateFace()
		{
			int facePoolSize = FacePoolSize;
			ConvexFaceInternal convexFaceInternal = new ConvexFaceInternal(Dimension, facePoolSize, GetVertexBuffer());
			FacePoolSize++;
			if (FacePoolSize > FacePoolCapacity)
			{
				ReallocateFacePool();
			}
			FacePool[facePoolSize] = convexFaceInternal;
			return facePoolSize;
		}

		public int GetFace()
		{
			if (FreeFaceIndices.Count > 0)
			{
				return FreeFaceIndices.Pop();
			}
			return CreateFace();
		}

		public void DepositConnector(FaceConnector connector)
		{
			if (ConnectorStack == null)
			{
				connector.Next = null;
				ConnectorStack = connector;
			}
			else
			{
				connector.Next = ConnectorStack;
				ConnectorStack = connector;
			}
		}

		public FaceConnector GetConnector()
		{
			if (ConnectorStack == null)
			{
				return new FaceConnector(Dimension);
			}
			FaceConnector connectorStack = ConnectorStack;
			ConnectorStack = ConnectorStack.Next;
			connectorStack.Next = null;
			return connectorStack;
		}

		public void DepositVertexBuffer(IndexBuffer buffer)
		{
			buffer.Clear();
			EmptyBufferStack.Push(buffer);
		}

		public IndexBuffer GetVertexBuffer()
		{
			if (EmptyBufferStack.Count == 0)
			{
				return new IndexBuffer();
			}
			return EmptyBufferStack.Pop();
		}

		public void DepositDeferredFace(DeferredFace face)
		{
			DeferredFaceStack.Push(face);
		}

		public DeferredFace GetDeferredFace()
		{
			if (DeferredFaceStack.Count == 0)
			{
				return new DeferredFace();
			}
			return DeferredFaceStack.Pop();
		}
	}
}
