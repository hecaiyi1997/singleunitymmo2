using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace UnityMMO
{
    public class WorldManager
    {
        public static WorldManager currinst = null;
        public int chunkSplit = 10;
        public float chunkSize = 200;
		ChunkPos lastChunkPos;
		public bool[,,] hasTerrainObjs = null;
		public UnityEngine.GameObject[,,] terrainObjs = null;
		public bool[,,] persistentterrainObjs = null;//记录需要加载的monster地图，加载完设置false
		public bool[,,] persistentObjs = null;//记录需要加载的monster地图，加载完还是true

		public bool[,,] haspreload = null;//记录已经提交monster地图加载的
		public Scene scene;
		public bool beganloadpersistent = false;//已经开始加载怪物的地图
		public struct ChunkPos
        {
            public int x;
            public int y;
        };
		public WorldManager()
		{
			currinst = this;

			lastChunkPos.x = -1;
			lastChunkPos.y = -1;
			hasTerrainObjs = new bool[chunkSplit, chunkSplit, 1];
			persistentterrainObjs = new bool[chunkSplit, chunkSplit, 1];
			haspreload = new bool[chunkSplit, chunkSplit, 1];
			persistentObjs = new bool[chunkSplit, chunkSplit, 1];
			for (int i = 0; i < chunkSplit; i++)
			{
				for (int ii = 0; ii < chunkSplit; ii++)
				{
					hasTerrainObjs[i, ii, 0] = false;
					persistentterrainObjs[i, ii, 0] = false;
					haspreload[i, ii, 0] = false;
					persistentObjs[i, ii, 0] = false;
				}
			}
			terrainObjs = new UnityEngine.GameObject[chunkSplit, chunkSplit, 1];
		}
		public  ChunkPos calcAtChunk(float x, float z)
        {
            ChunkPos pos;
            pos.x = (int)((x) / chunkSize); //发现地图的原点在（-278，-182，-67），所有当玩家位置（-278，xxoo，-67）相当于在（0，0）
			pos.y = (int)((z )/ chunkSize);//发现（0,1）等于（-278，xxoo，133）所以跟坐标轴一致

            if (pos.x >= chunkSplit)
                pos.x = -1;

            if (pos.y >= chunkSplit)
                pos.y = -1;

            return pos;
        }
		public bool waituntilallpersistentloaded()
		{
			
			if (beganloadpersistent == false) return false;
			for (int i = 0; i < chunkSplit; i++)
			{
				for (int ii = 0; ii < chunkSplit; ii++)
				{
					if (persistentterrainObjs[i, ii, 0] == true){//如果我是需要的monster地图，
						if (hasTerrainObjs[i, ii, 0] == false)
						{
							Debug.Log("loadpersistentChunks waituntilallpersistentloaded wait load" + i + ii);
							return false;//但是没有加载
						}
					}

				}

			}
			return true;

		}
		public void RecordpersistentChunks(Vector3 pos)
        {
			beganloadpersistent = true;
			ChunkPos chunkIdx = calcAtChunk(pos.x, pos.z);
			persistentterrainObjs[chunkIdx.x, chunkIdx.y, 0] = true;//记录等待加载的
			persistentObjs[chunkIdx.x, chunkIdx.y, 0] = true;
		}
		public IEnumerator loadpersistentChunks(Vector3 pos)
        {
			Debug.Log("loadpersistentChunks");
			ChunkPos chunkIdx = calcAtChunk(pos.x, pos.z);
			addLoadChunk(chunkIdx);
			Debug.Log("loadpersistentChunks1=" + chunkIdx.x + chunkIdx.y);
			yield return new WaitUntil(()=>hasTerrainObjs[chunkIdx.x, chunkIdx.y, 0]);
			Debug.Log("loadpersistentChunks2=" + chunkIdx.x + chunkIdx.y);
		}
		public void loadCurrentViewChunks(Vector3 pos)
		{

			Debug.Log("loadCurrentViewChunks" + pos.x + pos.z);
			ChunkPos currentPos = calcAtChunk(pos.x,pos.z);
			Debug.Log("loadCurrentViewChunks currentPos" + currentPos.x + currentPos.y);
			if (lastChunkPos.x == currentPos.x && lastChunkPos.y == currentPos.y)
				return;

			lastChunkPos = currentPos;

			// center
			//addLoadChunk(currentPos);
			for (int i = 0; i < chunkSplit; i++)
			{
				for (int ii = 0; ii < chunkSplit; ii++)
				{
					int xdiff = Mathf.Abs(i - currentPos.x);
					int ydiff = Mathf.Abs(ii - currentPos.y);
					if (xdiff <= 1 && ydiff <= 1)
					{
						if (hasTerrainObjs[i, ii, 0] == true)
						{
							continue;
						}

						ChunkPos cpos;
						cpos.x = i;
						cpos.y = ii;
						addLoadChunk(cpos);
					}
					else
					{
						//if (xdiff <= 2 && ydiff <= 2)
						//{
						//continue;
						//}
						if (persistentObjs[i, ii, 0])//是怪物地图不卸载
                        {
							continue;
						}
						if (hasTerrainObjs[i, ii, 0] == true)
						{
							hasTerrainObjs[i, ii, 0] = false;
							haspreload[i, ii, 0] = false;
							GameObject obj = terrainObjs[i, ii, 0];
							if (obj != null)
							{
								GameObject.Destroy(obj);
							}

							terrainObjs[i, ii, 0] = null;
						}
					}
				}
			}

		}
		public void Loadallchunk()
		{
			for (int i = 0; i < chunkSplit; i++)
			{
				for (int ii = 0; ii < chunkSplit; ii++)
				{
					ChunkPos cpos;
					cpos.x = i;
					cpos.y = ii;
					addLoadChunk(cpos);

				}
			}
		}
		public void addLoadChunk(ChunkPos chunkIdx)
		{
			if (chunkIdx.x < 0 || chunkIdx.y < 0 || chunkIdx.x >= chunkSplit || chunkIdx.y >= chunkSplit)
				return;

			if (hasTerrainObjs[chunkIdx.x, chunkIdx.y, 0] == true)
				return;
			if (haspreload[chunkIdx.x, chunkIdx.y, 0] == true)
				return;
			haspreload[chunkIdx.x, chunkIdx.y, 0] = true;
			string path = "Assets/AssetBundleRes/SplitPrefabs/terrainprefabs/" + getChunkName(chunkIdx) + ".prefab";

			XLuaFramework.ResourceManager.GetInstance().LoadAsset<GameObject>(path, delegate (UnityEngine.Object[] objs) {
                if (objs != null && objs.Length > 0)
                {
                    GameObject bodyObj = objs[0] as GameObject;
					GameObject go = GameObject.Instantiate(bodyObj);
					//List<GameObject> rootGameObjects = new List<GameObject>();
					//scene.GetRootGameObjects(rootGameObjects);
					//GameObject goo = rootGameObjects.Find(o => o.name == "GameObject");
					//go.transform.parent =goo.transform;
					hasTerrainObjs[chunkIdx.x, chunkIdx.y, 0] = true;
					terrainObjs[chunkIdx.x, chunkIdx.y, 0] = go;
					//persistentObjs[chunkIdx.x, chunkIdx.y, 0] = go;
					persistentterrainObjs[chunkIdx.x, chunkIdx.y, 0] = false;//已经加载设置false，用于查询全部完成
	
					Debug.Log("loadCurrentViewChunks addLoadChunk okloadpersistentChunks" + chunkIdx.x + chunkIdx.y + path);
				}
                else
                {
                    Debug.LogError("cannot fine file " + path);
                }
            });

			
		}

		public string getChunkName(ChunkPos chunkIdx)
		{
			return "Terrain"+ (chunkIdx.x) + "_" + (chunkIdx.y);
		}
		// Start is called before the first frame update
		void Start()
        {

        }

		// Update is called once per frame
		public void Update(Vector3 Playerpos)
        {
			loadCurrentViewChunks(Playerpos);
		}
    }
}
