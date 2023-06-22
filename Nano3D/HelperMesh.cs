using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.Geometry;
using Rhino.Input.Custom;
using Rhino;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nano3D
{
    internal class HelperMesh
    {
        /// <summary>
        /// A simple method to prompt the user to select a single mesh.
        /// </summary>
        /// <returns>Selected object that should be of type mesh.</returns>
        public static RhinoObject GetSingle(String message = "Select a single mesh")
        {
            GetObject go = new GetObject();
            go.SetCommandPrompt(message);
            go.GeometryFilter = ObjectType.Mesh;
            go.Get();
            if (go.CommandResult() != Result.Success) return null;
            if (go.ObjectCount != 1) return null;
            RhinoObject obj = go.Object(0).Object();
            if (obj.ObjectType != ObjectType.Mesh) return null;
            return obj;
        }

        public static void GetBuffers(Mesh mesh, out float[] vertexBuffer, out int[] indexBuffer)
        {
            // Convert quads to triangles
            mesh.Faces.ConvertQuadsToTriangles();

            // Get vertex buffer
            Point3f[] vertices = mesh.Vertices.ToPoint3fArray();
            vertexBuffer = new float[vertices.Length * 3];
            for (int i = 0; i < vertices.Length; i++)
            {
                vertexBuffer[i * 3] = vertices[i].X;
                vertexBuffer[i * 3 + 1] = vertices[i].Y;
                vertexBuffer[i * 3 + 2] = vertices[i].Z;
            }

            // Get index buffer
            MeshFace[] faces = mesh.Faces.ToArray();
            indexBuffer = new int[faces.Length * 3];
            for (int i = 0; i < faces.Length; i++)
            {
                MeshFace face = faces[i];
                indexBuffer[i * 3] = face.A;
                indexBuffer[i * 3 + 1] = face.B;
                indexBuffer[i * 3 + 2] = face.C;
            }
        }

        public static Mesh CreateFromBuffers(float[] vertexBuffer, int[] indexBuffer)
        {
            // Create new mesh
            Mesh newMesh = new Mesh();

            // Set vertices
            for (int i = 0; i < vertexBuffer.Length; i += 3)
            {
                Point3f vertex = new Point3f(vertexBuffer[i], vertexBuffer[i + 1], vertexBuffer[i + 2]);
                newMesh.Vertices.Add(vertex);
            }

            // Set faces
            for (int i = 0; i < indexBuffer.Length; i += 3)
            {
                int indexA = indexBuffer[i];
                int indexB = indexBuffer[i + 1];
                int indexC = indexBuffer[i + 2];
                newMesh.Faces.AddFace(indexA, indexB, indexC);
            }

            // Compute vertex normals
            newMesh.Normals.ComputeNormals();

            // Optional: Compact the mesh to remove unused vertices
            newMesh.Compact();

            // Optional: Ensure the mesh is valid
            bool valid = newMesh.IsValidWithLog(out string log);
            if (!valid)
            {
                RhinoApp.WriteLine("Warning: New mesh is not valid.");
                RhinoApp.WriteLine(log);
            }

            return newMesh;
        }

        public static void SaveAsStl(Mesh mesh, string fileName)
        {
            // Extract vertex buffer and index buffer.
            float[] vertexBuffer;
            int[] indexBuffer;
            GetBuffers(mesh, out vertexBuffer, out indexBuffer);

            SaveBuffersAsStl(vertexBuffer, indexBuffer, fileName);
        }

        public static void SaveBuffersAsStl(float[] vertexBuffer, int[] indexBuffer, string fileName)
        {
            // Open the file for writing
            using (FileStream fileStream = new FileStream(fileName, FileMode.Create))
            {
                // Write the STL header
                byte[] header = new byte[80];
                fileStream.Write(header, 0, header.Length);

                // Write the number of triangles
                int triangleCount = indexBuffer.Length / 3;
                byte[] triangleCountBytes = BitConverter.GetBytes(triangleCount);
                fileStream.Write(triangleCountBytes, 0, 4);

                // Write the triangles
                for (int i = 0; i < indexBuffer.Length; i += 3)
                {
                    // Get vertices for the current triangle
                    float x1 = vertexBuffer[indexBuffer[i] * 3];
                    float y1 = vertexBuffer[indexBuffer[i] * 3 + 1];
                    float z1 = vertexBuffer[indexBuffer[i] * 3 + 2];
                    float x2 = vertexBuffer[indexBuffer[i + 1] * 3];
                    float y2 = vertexBuffer[indexBuffer[i + 1] * 3 + 1];
                    float z2 = vertexBuffer[indexBuffer[i + 1] * 3 + 2];
                    float x3 = vertexBuffer[indexBuffer[i + 2] * 3];
                    float y3 = vertexBuffer[indexBuffer[i + 2] * 3 + 1];
                    float z3 = vertexBuffer[indexBuffer[i + 2] * 3 + 2];

                    // Compute the normal vector of the triangle
                    float nx = (y2 - y1) * (z3 - z1) - (z2 - z1) * (y3 - y1);
                    float ny = (z2 - z1) * (x3 - x1) - (x2 - x1) * (z3 - z1);
                    float nz = (x2 - x1) * (y3 - y1) - (y2 - y1) * (x3 - x1);
                    float length = (float)Math.Sqrt(nx * nx + ny * ny + nz * nz);
                    nx /= length;
                    ny /= length;
                    nz /= length;

                    // Write the normal vector
                    byte[] normal = new byte[12];
                    BitConverter.GetBytes(nx).CopyTo(normal, 0);
                    BitConverter.GetBytes(ny).CopyTo(normal, 4);
                    BitConverter.GetBytes(nz).CopyTo(normal, 8);
                    fileStream.Write(normal, 0, normal.Length);

                    // Write the vertices in counter-clockwise order
                    byte[] triangle = new byte[36];
                    BitConverter.GetBytes(x1).CopyTo(triangle, 12);
                    BitConverter.GetBytes(y1).CopyTo(triangle, 16);
                    BitConverter.GetBytes(z1).CopyTo(triangle, 20);
                    BitConverter.GetBytes(x3).CopyTo(triangle, 0);
                    BitConverter.GetBytes(y3).CopyTo(triangle, 4);
                    BitConverter.GetBytes(z3).CopyTo(triangle, 8);
                    BitConverter.GetBytes(x2).CopyTo(triangle, 24);
                    BitConverter.GetBytes(y2).CopyTo(triangle, 28);
                    BitConverter.GetBytes(z2).CopyTo(triangle, 32);
                    fileStream.Write(triangle, 0, triangle.Length);

                    // Write the triangle attribute (zero)
                    byte[] attribute = new byte[2];
                    fileStream.Write(attribute, 0, attribute.Length);
                }
            }
        }

        /// <summary>
        /// Prepare an array of bytes expected by the Nano3D service.
        /// </summary>
        /// <returns>
        /// Returned data structure is like this: https://stackoverflow.com/a/75432567/3405291
        /// </returns>
        public static byte[] ProcessBuffers(int[] indexBuffer, float[] vertexBuffer)
        {
            // Compute the length of the index buffer in bytes
            int indexBufferSize = sizeof(int) * indexBuffer.Length;

            // Compute the length of the vertex buffer in bytes
            int vertexBufferSize = sizeof(float) * vertexBuffer.Length;

            // Create a new array of bytes to hold the packed index and vertex data
            byte[] data = new byte[sizeof(int) * 2 + indexBufferSize + vertexBufferSize];

            // Pack the length of the index buffer into the data array
            byte[] indexLengthBytes = BitConverter.GetBytes(indexBufferSize);
            Array.Copy(indexLengthBytes, 0, data, 0, sizeof(int));

            // Pack the index buffer into the data array
            for (int i = 0; i < indexBuffer.Length; i++)
            {
                byte[] indexBytes = BitConverter.GetBytes(indexBuffer[i]);
                Array.Copy(indexBytes, 0, data, sizeof(int) + i * sizeof(int), sizeof(int));
            }

            // Pack the length of the vertex buffer into the data array
            byte[] vertexLengthBytes = BitConverter.GetBytes(vertexBufferSize);
            Array.Copy(vertexLengthBytes, 0, data, sizeof(int) + indexBufferSize, sizeof(int));

            // Pack the vertex buffer into the data array
            for (int i = 0; i < vertexBuffer.Length; i++)
            {
                byte[] vertexBytes = BitConverter.GetBytes(vertexBuffer[i]);
                Array.Copy(vertexBytes, 0, data, sizeof(int) * 2 + indexBufferSize + i * sizeof(float), sizeof(float));
            }

            return data;
        }

        public static void UnpackBuffers(byte[] data, out int[] indexBuffer, out float[] vertexBuffer)
        {
            // Read the length of the index buffer from the data array
            int indexBufferLength = BitConverter.ToInt32(data, 0);

            // Read the index buffer from the data array
            indexBuffer = new int[indexBufferLength / sizeof(int)];
            for (int i = 0; i < indexBuffer.Length; i++)
            {
                indexBuffer[i] = BitConverter.ToInt32(data, sizeof(int) + i * sizeof(int));
            }

            // Read the length of the vertex buffer from the data array
            int vertexBufferLength = BitConverter.ToInt32(data, sizeof(int) + indexBufferLength);

            // Read the vertex buffer from the data array
            vertexBuffer = new float[vertexBufferLength / sizeof(float)];
            for (int i = 0; i < vertexBuffer.Length; i++)
            {
                vertexBuffer[i] = BitConverter.ToSingle(data, sizeof(int) * 2 + indexBufferLength + i * sizeof(float));
            }
        }

        public static bool HasInvalidVertexIndices(Mesh mesh)
        {
            // Check each face in the mesh
            for (int i = 0; i < mesh.Faces.Count; i++)
            {
                MeshFace face = mesh.Faces[i];

                // Check each vertex index in the face
                if (face.A < 0 || face.A >= mesh.Vertices.Count ||
                    face.B < 0 || face.B >= mesh.Vertices.Count ||
                    face.C < 0 || face.C >= mesh.Vertices.Count)
                {
                    // If any vertex index is invalid, return true
                    return true;
                }
            }

            // If all face vertex indices are valid, return false
            return false;
        }

        public static void PrintFaceVertices(Mesh mesh, int faceIndex)
        {
            if (faceIndex >= mesh.Faces.Count)
            {
                RhinoApp.WriteLine("Invalid face index.");
                return;
            }

            MeshFace face = mesh.Faces[faceIndex];
            RhinoApp.WriteLine("Indices of face {0}:", faceIndex);
            RhinoApp.WriteLine("Index A: {0}", face.A);
            RhinoApp.WriteLine("Index B: {0}", face.B);
            RhinoApp.WriteLine("Index C: {0}", face.C);
            RhinoApp.WriteLine("Vertices of face {0}:", faceIndex);
            RhinoApp.WriteLine("Vertex A: {0}", mesh.Vertices[face.A]);
            RhinoApp.WriteLine("Vertex B: {0}", mesh.Vertices[face.B]);
            RhinoApp.WriteLine("Vertex C: {0}", mesh.Vertices[face.C]);
        }

        // To resolve a problem with the output mesh of algorithms:
        // https://discourse.mcneel.com/t/warning-new-mesh-is-not-valid-on-mesh-m-f-vi-has-invalid-vertex-indices/159312/17?u=megidd_git
        public static int[] RemoveZeroAreaTriangles(int[] indexBuffer, float[] vertexBuffer)
        {
            // Remove the zero-area triangles from the index buffer
            List<int> newIndexBuffer = new List<int>();
            for (int i = 0; i < indexBuffer.Length; i += 3)
            {
                // Get the vertex positions for the three vertices of the triangle
                int indexA = indexBuffer[i];
                int indexB = indexBuffer[i + 1];
                int indexC = indexBuffer[i + 2];
                Vector3d vertexA = new Vector3d(vertexBuffer[indexA * 3], vertexBuffer[indexA * 3 + 1], vertexBuffer[indexA * 3 + 2]);
                Vector3d vertexB = new Vector3d(vertexBuffer[indexB * 3], vertexBuffer[indexB * 3 + 1], vertexBuffer[indexB * 3 + 2]);
                Vector3d vertexC = new Vector3d(vertexBuffer[indexC * 3], vertexBuffer[indexC * 3 + 1], vertexBuffer[indexC * 3 + 2]);

                // Compute the cross product of two edges of the triangle
                Vector3d edge1 = vertexB - vertexA;
                Vector3d edge2 = vertexC - vertexA;
                Vector3d crossProduct = Vector3d.CrossProduct(edge1, edge2);

                // Check if the cross product has zero length
                if (crossProduct.Length < RhinoMath.ZeroTolerance)
                {
                    // If the cross product has zero length, skip this triangle
                    continue;
                }

                // Add the indices of the non-zero-area triangle to the new index buffer
                newIndexBuffer.Add(indexA);
                newIndexBuffer.Add(indexB);
                newIndexBuffer.Add(indexC);
            }

            // Convert the new index buffer to an array and return it
            return newIndexBuffer.ToArray();
        }
    }
}
