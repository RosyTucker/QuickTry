using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using Windows.Kinect;
using Assets.Lib.Models;
using UnityEngine;

namespace Assets.Lib.Parser
{
    public class ClothingParser
    {
        public static Dictionary<string, ClothingItem> Parse(string filePath)
        {
            var clothingFile = (TextAsset)Resources.Load(filePath);
            var clothingXml = new XmlDocument();
            clothingXml.LoadXml(clothingFile.text);
            var itemsXml = clothingXml.GetElementsByTagName(XmlTags.Item);

            return itemsXml.Cast<XmlNode>()
                .Select<XmlNode, ClothingItem>(ParseClothingItem)
                .ToDictionary(item => item.Id);
        }

        private static ClothingItem ParseClothingItem(XmlNode item)
        {
            var childNodes = item.ChildNodes.Cast<XmlNode>().ToList();

            var idNode = childNodes.FirstOrDefault(node => node.Name == XmlTags.ItemId);
            if (idNode == null) throw new FormatException("Item is Missing Id");

            var typeNode = childNodes.FirstOrDefault(node => node.Name == XmlTags.ItemType);
            if (typeNode == null) throw new FormatException("Item is Missing Type");

            var textureNode = childNodes.FirstOrDefault(node => node.Name == XmlTags.ItemTexture);
            if (textureNode == null) throw new XmlSchemaException("Item is Missing " + XmlTags.ItemTexture);

            var materialNode = childNodes.FirstOrDefault(node => node.Name == XmlTags.ItemMaterial);
            if (materialNode == null) throw new XmlSchemaException("Item is Missing " + XmlTags.ItemMaterial);

            var meshNode = childNodes.FirstOrDefault(node => node.Name == XmlTags.ItemMesh);
            if (meshNode == null) throw new XmlSchemaException("Item is Missing " + XmlTags.ItemMesh);

            var baseSizeNode = childNodes.FirstOrDefault(node => node.Name == XmlTags.ItemBaseSize);
            if (baseSizeNode == null) throw new XmlSchemaException("Item is Missing " + XmlTags.ItemBaseSize);

            var baseYRotationNode = childNodes.FirstOrDefault(node => node.Name == XmlTags.ItemBaseYRotation);
            if (baseYRotationNode == null) throw new XmlSchemaException("Item is Missing " + XmlTags.ItemBaseYRotation);


            var id = idNode.InnerText;
            var type = typeNode.InnerText.ToEnum<ClothingType>();
            var texture = textureNode.InnerText;
            var material = materialNode.InnerText;
            var mesh = meshNode.InnerText;
            var baseSize = float.Parse(baseSizeNode.InnerText);
            var baseYRotation = int.Parse(baseYRotationNode.InnerText);

            return new ClothingItem(id, type, texture, mesh, material, baseSize, baseYRotation);
        }


    }
}