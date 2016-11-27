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

            var attachmentPointsNode = childNodes.FirstOrDefault(node => node.Name == XmlTags.ItemAttachmentPoints);
            if (attachmentPointsNode == null) throw new FormatException("Item is Missing " + XmlTags.ItemAttachmentPoints);

            var pointNodes = attachmentPointsNode.ChildNodes;
            var attachmentPoints = pointNodes.Cast<XmlNode>()
             .Select<XmlNode, ClothingAttachmentPoint>(ParseClothingAttachmentPoint)
             .ToDictionary(point => point.Type);

            var id = idNode.InnerText;
            var type = typeNode.InnerText.ToEnum<ClothingType>();
            var texture = textureNode.InnerText;
            var material = materialNode.InnerText;
            var mesh = meshNode.InnerText;

            return new ClothingItem(id, type, texture, mesh, material, attachmentPoints);
        }

        private static ClothingAttachmentPoint ParseClothingAttachmentPoint(XmlNode attachmentNode)
        {
            var childNodes = attachmentNode.ChildNodes.Cast<XmlNode>().ToList();

            var typeNode = childNodes.FirstOrDefault(node => node.Name == XmlTags.AttachmentPointType);
            if (typeNode == null) throw new FormatException("AttachmentPoint is Missing " + XmlTags.AttachmentPointType);

            var xCoordNode = childNodes.FirstOrDefault(node => node.Name == XmlTags.AttachmentPointX);
            if (xCoordNode == null) throw new XmlSchemaException("AttachmentPoint is Missing " + XmlTags.AttachmentPointX);

            var yCoordNode = childNodes.FirstOrDefault(node => node.Name == XmlTags.AttachmentPointX);
            if (yCoordNode == null) throw new XmlSchemaException("AttachmentPoint is Missing " + XmlTags.AttachmentPointY);

            var zCoordNode = childNodes.FirstOrDefault(node => node.Name == XmlTags.AttachmentPointX);
            if (zCoordNode == null) throw new XmlSchemaException("AttachmentPoint is Missing " + XmlTags.AttachmentPointZ);

            var type = typeNode.InnerText.ToEnum<JointType>();
            var xCoord = int.Parse(xCoordNode.InnerText);
            var yCoord = int.Parse(yCoordNode.InnerText);
            var zCoord = int.Parse(zCoordNode.InnerText);

            return new ClothingAttachmentPoint(type, new Vector3(xCoord, yCoord, zCoord));
        }
    }
}