using System.Collections.Generic;
using UnityEngine;

namespace AStar
{

    public static class Floyd
    {
        #region floyd
        //----------------------------------------弗洛伊德路径平滑--------------------------------------//

        public static List<Vector3> FloydSmooth(List<Vector3> path, Grid grid)
        {
            if (path == null)
            {
                return path;
            }


            int len = path.Count;
            //去掉同一条线上的点。
            if (len > 2)
            {
                Vector3 vector = path[len - 1] - path[len - 2];
                Vector3 tempvector;
                for (int i = len - 3; i >= 0; i--)
                {
                    tempvector = path[i + 1] - path[i];
                    if (Vector3.Cross(vector, tempvector).y == 0f)
                    {
                        path.RemoveAt(i + 1);
                    }
                    else
                    {
                        vector = tempvector;
                    }
                }
            }
            //去掉无用拐点
            len = path.Count;
            for (int i = len - 1; i >= 0; i--)
            {
                for (int j = 0; j <= i - 1; j++)
                {
                    if (CheckCrossNoteWalkable(path[i], path[j], grid))
                    {
                        for (int k = i - 1; k >= j; k--)
                        {
                            path.RemoveAt(k);
                        }
                        i = j;
                        //len = path.Count;
                        break;
                    }
                }
            }
            return path;
        }
        #endregion end floyd

        /// <summary>
        /// 检测两点是否可以通过
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static bool CheckCrossNoteWalkable(Vector3 p1, Vector3 p2, Grid grid)
        {
            return false;
        }

        /*
        static float currentY; // 用于检测攀爬与下落高度
        static float Tilesize; // 用于检测攀爬与下落高度
        static float ClimbLimit; // 用于检测攀爬与下落高度
        static Vector3 MapStartPosition;
        static float MaxFalldownHeight;
        //判断路径上是否有障碍物
        public static bool CheckCrossNoteWalkable1(Vector3 p1, Vector3 p2)
        {
            currentY = p1.y; //记录初始高度，用于检测是否可通过
            bool changexz = Mathf.Abs(p2.z - p1.z) > Mathf.Abs(p2.x - p1.x);
            if (changexz)
            {
                float temp = p1.x;
                p1.x = p1.z;
                p1.z = temp;
                temp = p2.x;
                p2.x = p2.z;
                p2.z = temp;
            }
            if (!Checkwalkable(changexz, p1.x, p1.z))
            {
                return false;
            }
            float stepX = p2.x > p1.x ? Tilesize : (p2.x < p1.x ? -Tilesize : 0);
            float stepY = p2.y > p1.y ? Tilesize : (p2.y < p1.y ? -Tilesize : 0);
            float deltay = Tilesize * ((p2.z - p1.z) / Mathf.Abs(p2.x - p1.x));
            float nowX = p1.x + stepX / 2;
            float nowY = p1.z - stepY / 2;
            float CheckY = nowY;

            while (nowX != p2.x)
            {
                if (!Checkwalkable(changexz, nowX, CheckY))
                {
                    return false;
                }
                nowY += deltay;
                if (nowY >= CheckY + stepY)
                {
                    CheckY += stepY;
                    if (!Checkwalkable(changexz, nowX, CheckY))
                    {
                        return false;
                    }
                }
                nowX += stepX;
            }
            return true;
        }
        private static bool Checkwalkable(bool changeXZ, float x, float z)
        {
            int mapx = (MapStartPosition.x < 0F) ? Mathf.FloorToInt(((x + Mathf.Abs(MapStartPosition.x)) / Tilesize)) : Mathf.FloorToInt((x - MapStartPosition.x) / Tilesize);
            int mapz = (MapStartPosition.y < 0F) ? Mathf.FloorToInt(((z + Mathf.Abs(MapStartPosition.y)) / Tilesize)) : Mathf.FloorToInt((z - MapStartPosition.y) / Tilesize);
            if (mapx < 0 || mapz < 0 || mapx >= Map.GetLength(0) || mapz >= Map.GetLength(1))
            {
                return false;
            }

            Node note;
            if (changeXZ)
            {
                note = Map[mapz, mapx];
            }
            else
            {
                note = Map[mapx, mapz];
            }
            bool ret = note.walkable && ((note.gridY - currentY <= ClimbLimit && note.gridY >= currentY) || (currentY - note.gridY <= MaxFalldownHeight && currentY >= note.gridY));
            if (ret)
            {
                currentY = note.gridY;
            }
            return ret;
        }
        */
    }
}