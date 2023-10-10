using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;


public class TerrainSplitEditor2 : EditorWindow
{
    public static string TerrainSavePath = "Assets/AssetBundleRes/SplitTerrain2/";
    //分割大小
    public static int SLICING_SIZE = 10;

    public static UnityEngine.GameObject[,,] terrainObjs = null;
    [MenuItem("Split/Split Terrain2")]
    private static void Slicing()
    {
        Terrain terrain = GameObject.FindObjectOfType<Terrain>();
        //terrainObjs = new UnityEngine.GameObject[10, 10, 1];
        if (terrain == null)
        {
            Debug.LogError("找不到地形!"); return;
        }
        if (Directory.Exists(TerrainSavePath)) Directory.Delete(TerrainSavePath, true);
        Directory.CreateDirectory(TerrainSavePath);
        TerrainData terrainData = terrain.terrainData;
        //这里我分割的宽和长度是一样的.这里求出循环次数,TerrainLoad.SIZE 要生成的地形宽度,长度相同
        //高度地图的分辨率只能是 2 的 N 次幂加 1,所以 SLICING_SIZE 必须为 2 的 N 次幂
        SLICING_SIZE = (int)terrainData.size.x / 200;
        Vector3 oldSize = terrainData.size;
        //得到新地图分辨率
        int newHeightmapResolution = (terrainData.heightmapResolution - 1) / SLICING_SIZE;
        int newAlphamapResolution = terrainData.alphamapResolution / SLICING_SIZE;
        int newbaseMapResolution = terrainData.baseMapResolution / SLICING_SIZE;
        SplatPrototype[] splatProtos = terrainData.splatPrototypes;
        //循环宽和长,生成小块地形
        for (int x = 0; x < SLICING_SIZE; ++x)
        {
            for (int y = 0; y < SLICING_SIZE; ++y)
            {
                //创建资源
                TerrainData newData = new TerrainData(); string terrainName = TerrainSavePath + "Terrain" + x + "_" + y + ".asset";
                AssetDatabase.CreateAsset(newData, TerrainSavePath + "Terrain" + x + "_" + y + ".asset");
                EditorUtility.DisplayProgressBar(" 正 在分 割 地形 ", terrainName, (float)(x * SLICING_SIZE + y) /
                (float)(SLICING_SIZE * SLICING_SIZE));
                //设置分辨率参数
                newData.heightmapResolution = (terrainData.heightmapResolution - 1) / SLICING_SIZE;
                newData.alphamapResolution = terrainData.alphamapResolution / SLICING_SIZE;
                newData.baseMapResolution = terrainData.baseMapResolution / SLICING_SIZE;
                //设置大小
                //newData.size = new Vector3(oldSize.x / SLICING_SIZE, oldSize.y, oldSize.z / SLICING_SIZE);
                //设置地形原型
                SplatPrototype[] newSplats = new SplatPrototype[splatProtos.Length];
                for (int i = 0; i < splatProtos.Length; ++i)
                {
                    newSplats[i] = new SplatPrototype(); newSplats[i].texture = splatProtos[i].texture; newSplats[i].tileSize = splatProtos[i].tileSize;
                    float offsetX = (newData.size.x * x) % splatProtos[i].tileSize.x + splatProtos[i].tileOffset.x;
                    float offsetY = (newData.size.z * y) % splatProtos[i].tileSize.y + splatProtos[i].tileOffset.y;
                    newSplats[i].tileOffset = new Vector2(offsetX, offsetY);
                }
                newData.splatPrototypes = newSplats;
                //设置混合贴图
                float[,,] alphamap = new float[newAlphamapResolution, newAlphamapResolution, splatProtos.Length];
                alphamap = terrainData.GetAlphamaps(x * newData.alphamapWidth, y * newData.alphamapHeight, newData.alphamapWidth, newData.alphamapHeight);
                newData.SetAlphamaps(0, 0, alphamap);
                //设置高度
                int xBase = terrainData.heightmapResolution / SLICING_SIZE;
                int yBase = terrainData.heightmapResolution / SLICING_SIZE;
                float[,] height = terrainData.GetHeights(xBase * x, yBase * y, xBase + 1, yBase + 1);
                newData.SetHeights(0, 0, height);
                
                /*GameObject gameObject = Terrain.CreateTerrainGameObject(newData);
                float xMin = terrain.terrainData.size.x / 10 * x;
                float yMin = terrain.terrainData.size.x / 10 * y;

                float xMax = terrain.terrainData.size.x / 10 * (x + 1);

                float yMax = terrain.terrainData.size.z / 10 * (y + 1);
                newData.size = new Vector3(xMax - xMin, terrain.terrainData.size.y, yMax - yMin);
                */
                //gameObject.transform.position = new Vector3(terrain.transform.position.x + xMin, terrain.transform.position.y, terrain.transform.position.z + yMin);
                //gameObject.transform.position = new Vector3(terrain.transform.position.x + xMin, 0, terrain.transform.position.z + yMin);
                //terrainObjs[x, y, 0] = gameObject;
                //gameObject.name = "Terrain" + x + "_" + y;

                
            }
        }
        EditorUtility.ClearProgressBar();
        //autoSetNeighbors();
    }
    public static void autoSetNeighbors()
    {
        int arrayPos = 0;
        int terrainsLong = 10;
        int terrainsWide = 10;

        Terrain[] terrains = new Terrain[terrainsLong * terrainsWide];

        for (int y = 0; y < terrainsLong; y++)
        {
            for (int x = 0; x < terrainsWide; x++)
            {
                if (terrainObjs[x, y, 0] != null)
                    terrains[arrayPos] = terrainObjs[x, y, 0].GetComponent<Terrain>();
                else
                    terrains[arrayPos] = null;

                arrayPos++;
            }
        }

        arrayPos = 0;
        for (int y = 0; y < terrainsLong; y++)
        {
            for (int x = 0; x < terrainsWide; x++)
            {
                if (terrains[arrayPos] == null)
                {
                    arrayPos++;
                    continue;
                }

                if (y == 0)
                {
                    if (x == 0)
                        terrains[arrayPos].SetNeighbors(null, terrains[arrayPos + terrainsWide], terrains[arrayPos + 1], null);
                    else if (x == terrainsWide - 1)
                        terrains[arrayPos].SetNeighbors(terrains[arrayPos - 1], terrains[arrayPos + terrainsWide], null, null);
                    else
                        terrains[arrayPos].SetNeighbors(terrains[arrayPos - 1], terrains[arrayPos + terrainsWide], terrains[arrayPos + 1], null);
                }
                else if (y == terrainsLong - 1)
                {
                    if (x == 0)
                        terrains[arrayPos].SetNeighbors(null, null, terrains[arrayPos + 1], terrains[arrayPos - terrainsWide]);
                    else if (x == terrainsWide - 1)
                        terrains[arrayPos].SetNeighbors(terrains[arrayPos - 1], null, null, terrains[arrayPos - terrainsWide]);
                    else
                        terrains[arrayPos].SetNeighbors(terrains[arrayPos - 1], null, terrains[arrayPos + 1], terrains[arrayPos - terrainsWide]);
                }
                else
                {
                    if (x == 0)
                        terrains[arrayPos].SetNeighbors(null, terrains[arrayPos + terrainsWide], terrains[arrayPos + 1], terrains[arrayPos - terrainsWide]);
                    else if (x == terrainsWide - 1)
                        terrains[arrayPos].SetNeighbors(terrains[arrayPos - 1], terrains[arrayPos + terrainsWide], null, terrains[arrayPos - terrainsWide]);
                    else
                        terrains[arrayPos].SetNeighbors(terrains[arrayPos - 1], terrains[arrayPos + terrainsWide], terrains[arrayPos + 1], terrains[arrayPos - terrainsWide]);
                }

                arrayPos++;
            }
        }

    }
}
