using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor;

public class TreeTest
{
    // To test if objects will collide with the tree's
    [Test]
    public void TreeTestSimplePasses()
    {
        //find the tree asset
        string[] tree = AssetDatabase.FindAssets("tree_detailed (1)");
        var asset = tree[0];
        var path = AssetDatabase.GUIDToAssetPath(asset);
        GameObject prefab = AssetDatabase.LoadAssetAtPath(path, typeof(Object)) as GameObject;
        //get the assets mesh colliders
        CapsuleCollider[] capsuleColliders = prefab.GetComponentsInChildren<CapsuleCollider>(true) as CapsuleCollider[];
        //check if the list contains mesh colliders
        Assert.IsTrue(capsuleColliders.Length>0);
        //check if the mesh colliders are enabled
        if(capsuleColliders.Length>0){
        foreach(CapsuleCollider capsuleCollider in capsuleColliders){
            Assert.IsTrue(capsuleCollider.enabled);
        }
        }
    }
}