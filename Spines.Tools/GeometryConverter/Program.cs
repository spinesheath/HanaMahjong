// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GeometryConverter
{
  internal class Program
  {
    private static readonly Regex VertexRegex = new Regex("v ([0-9.-]+) ([0-9.-]+) ([0-9.-]+)");
    private static readonly Regex VertexNormalRegex = new Regex("vn ([0-9.-]+) ([0-9.-]+) ([0-9.-]+)");
    private static readonly Regex UvRegex = new Regex("vt ([0-9.-]+) ([0-9.-]+)");
    private static readonly Regex FaceRegex = new Regex("f ([0-9]+)/([0-9]+)/([0-9]+) ([0-9]+)/([0-9]+)/([0-9]+) ([0-9]+)/([0-9]+)/([0-9]+)");

    private static void Main(string[] args)
    {
      Console.WriteLine("Source:");
      //var sourcePath = Console.ReadLine();
      const string sourcePath = @"D:\Johannes\Desktop\textures\tile.obj";
      Console.WriteLine("Target:");
      //var targetPath = Console.ReadLine();
      var targetPath = @"D:\Johannes\Desktop\textures\converted.json";

      var source = File.ReadAllLines(sourcePath);
      var vertices = GetVertices(source).ToList();
      var uvs = GetUvs(source).ToList();
      var normals = GetNormals(source).ToList();
      var faces = GetFaces(source).ToList();

      var json = new StringBuilder();
      json.AppendLine("{");
      json.AppendLine("\t\"metadata\": {");
      json.AppendLine("\t\t\"version\": 4.5,");
      json.AppendLine("\t\t\"type\": \"BufferGeometry\",");
      json.AppendLine("\t\t\"generator\": \"BufferGeometry.toJSON\"");
      json.AppendLine("\t},");
      json.AppendLine("\t\"uuid\": \"7DA049F2-FE7B-40BB-81F7-6C95C2B78165\",");
      json.AppendLine("\t\"type\": \"BufferGeometry\",");
      json.AppendLine("\t\"data\": {");
      json.AppendLine($"\t\t\"vertices\": [{string.Join(",", vertices)}],");
      json.AppendLine($"\t\t\"normals\": [{string.Join(",", normals)}],");
      json.AppendLine($"\t\t\"uvs\": [[{string.Join(",", uvs)}]],");
      json.AppendLine($"\t\t\"faces\": [{string.Join(",", faces)}]");
      json.AppendLine("\t}");
      json.AppendLine("}");

      File.WriteAllText(targetPath, json.ToString());

      Console.ReadKey();
    }

    private static IEnumerable<Face> GetFaces(string[] source)
    {
      foreach (var line in source)
      {
        var match = FaceRegex.Match(line);
        if (match.Success)
        {
          var vId1 = ConvertMinusOne(match, 1);
          var vId2 = ConvertMinusOne(match, 4);
          var vId3 = ConvertMinusOne(match, 7);
          var uvId1 = ConvertMinusOne(match, 2);
          var uvId2 = ConvertMinusOne(match, 5);
          var uvId3 = ConvertMinusOne(match, 8);
          var nId1 = ConvertMinusOne(match, 3);
          var nId2 = ConvertMinusOne(match, 6);
          var nId3 = ConvertMinusOne(match, 9);
          yield return new Face(vId1, vId2, vId3, uvId1, uvId2, uvId3, nId1, nId2, nId3);
        }
      }
    }


    private static int ConvertMinusOne(Match match, int groupId)
    {
      return Convert.ToInt32(match.Groups[groupId].Value, CultureInfo.InvariantCulture) - 1;
    }

    private static IEnumerable<Vec3> GetNormals(string[] source)
    {
      foreach (var line in source)
      {
        var match = VertexNormalRegex.Match(line);
        if (match.Success)
        {
          var x = Convert.ToDouble(match.Groups[1].Value, CultureInfo.InvariantCulture);
          var y = Convert.ToDouble(match.Groups[2].Value, CultureInfo.InvariantCulture);
          var z = Convert.ToDouble(match.Groups[3].Value, CultureInfo.InvariantCulture);
          yield return new Vec3(x, y, z);
        }
      }
    }

    private static IEnumerable<Vec2> GetUvs(string[] source)
    {
      foreach (var line in source)
      {
        var match = UvRegex.Match(line);
        if (match.Success)
        {
          var x = Convert.ToDouble(match.Groups[1].Value, CultureInfo.InvariantCulture);
          var y = Convert.ToDouble(match.Groups[2].Value, CultureInfo.InvariantCulture);
          yield return new Vec2(x, y);
        }
      }
    }

    private static IEnumerable<Vec3> GetVertices(string[] source)
    {
      foreach (var line in source)
      {
        var match = VertexRegex.Match(line);
        if (match.Success)
        {
          var x = Convert.ToDouble(match.Groups[1].Value, CultureInfo.InvariantCulture);
          var y = Convert.ToDouble(match.Groups[2].Value, CultureInfo.InvariantCulture);
          var z = Convert.ToDouble(match.Groups[3].Value, CultureInfo.InvariantCulture);
          yield return new Vec3(x, y, z);
        }
      }
    }
  }

  internal class Face
  {
    public Face(int vId1, int vId2, int vId3, int uvId1, int uvId2, int uvId3, int nId1, int nId2, int nId3)
    {
      VId1 = vId1;
      VId2 = vId2;
      VId3 = vId3;
      UvId1 = uvId1;
      UvId2 = uvId2;
      UvId3 = uvId3;
      NId1 = nId1;
      NId2 = nId2;
      NId3 = nId3;
    }

    public int VId1 { get; }
    public int VId2 { get; }
    public int VId3 { get; }
    public int UvId1 { get; }
    public int UvId2 { get; }
    public int UvId3 { get; }
    public int NId1 { get; }
    public int NId2 { get; }
    public int NId3 { get; }

    //public int Flags { get; } = 40;
    // No face vertex normals
    public int Flags { get; } = 8;

    public override string ToString()
    {
      return $"{Flags},{VId1},{VId2},{VId3},{UvId1},{UvId2},{UvId3}";
    }
  }

  internal class Vec2
  {
    public Vec2(double x, double y)
    {
      X = x;
      Y = y;
    }

    public double X { get; }
    public double Y { get; }

    public override string ToString()
    {
      return $"{X.ToString(CultureInfo.InvariantCulture)},{Y.ToString(CultureInfo.InvariantCulture)}";
    }
  }

  internal class Vec3
  {
    public Vec3(double x, double y, double z)
    {
      X = x;
      Y = y;
      Z = z;
    }

    public double X { get; }
    public double Y { get; }
    public double Z { get; }

    public override string ToString()
    {
      return $"{X.ToString(CultureInfo.InvariantCulture)},{Y.ToString(CultureInfo.InvariantCulture)},{Z.ToString(CultureInfo.InvariantCulture)}";
    }
  }
}