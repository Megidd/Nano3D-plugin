using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using Rhino;
using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.FileIO;
using Rhino.Geometry;
using Rhino.Geometry.Collections;
using Rhino.Input.Custom;
using Command = Rhino.Commands.Command;

namespace Nano3D
{
    public class Nano3DHollowing : Command
    {
        public Nano3DHollowing()
        {
            Instance = this;
        }

        ///<summary>The only instance of the MyCommand command.</summary>
        public static Nano3DHollowing Instance { get; private set; }

        public override string EnglishName => "Nano3DHollowing";

        private static readonly HttpClient client = new HttpClient();

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            RhinoApp.WriteLine("The {0} command received the document.", EnglishName);

            // TODO: complete command.

            RhinoObject obj = GetSingleMesh();
            if (null == obj || obj.ObjectType != ObjectType.Mesh)
            {
                RhinoApp.WriteLine("Mesh is not valid.");
                return Result.Failure;
            }
            Mesh mesh = obj.Geometry as Mesh;
            if (mesh == null)
            {
                RhinoApp.WriteLine("Mesh is not valid.");
                return Result.Failure;
            }
            bool converted = mesh.Faces.ConvertQuadsToTriangles();
            if (converted)
            {
                RhinoApp.WriteLine("Mesh contains quads. They are converted to triangles.");
            }
            RhinoApp.WriteLine("Number of mesh vertices: {0}", mesh.Vertices.Count);
            RhinoApp.WriteLine("Number of mesh triangles: {0}", mesh.Faces.Count);

            // Extract vertex buffer and index buffer.
            float[] vertexBuffer;
            int[] indexBuffer;
            GetMeshBuffers(mesh, out vertexBuffer, out indexBuffer);

            SaveBuffersAsStl(vertexBuffer, indexBuffer, "mesh-out.stl");

            RhinoApp.WriteLine("The {0} command finished.", EnglishName);
            return Result.Success;
        }

        // A simple method to prompt the user to select a single mesh and return it.
        public static RhinoObject GetSingleMesh()
        {
            GetObject go = new GetObject();
            go.SetCommandPrompt("Select a single mesh");
            go.GeometryFilter = ObjectType.Mesh;
            go.Get();
            if (go.CommandResult() != Result.Success) return null;
            if (go.ObjectCount != 1) return null;
            RhinoObject obj = go.Object(0).Object();
            if (obj.ObjectType != ObjectType.Mesh) return null;
            return obj;
        }

        public static void GetMeshBuffers(Mesh mesh, out float[] vertexBuffer, out int[] indexBuffer)
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

        public static Mesh CreateMesh(float[] vertexBuffer, int[] indexBuffer)
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
    }
}