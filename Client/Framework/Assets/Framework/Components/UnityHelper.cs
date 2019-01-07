using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Framework
{
    public class UnityHepler
    {
        static public void setLocalPositionByPivot(GameObject target, Vector3 newPosition, Vector3 pivot)
        {
            var rectTrans = target.GetComponent<RectTransform>();
            if (rectTrans == null)
            {
                Debug.LogError("setLocalPositionByArchor error: target dose not contain RectTransform !");
                return;
            }
            var newPivot = new Vector3(pivot.x - rectTrans.pivot.x, pivot.y - rectTrans.pivot.y, pivot.z);

            target.transform.localPosition = new Vector3(
                newPosition.x - rectTrans.rect.width * newPivot.x,
                newPosition.y - rectTrans.rect.height * newPivot.y,
                newPosition.z);
        }

        static public Vector3 getLocalPositioByPivot(GameObject target, Vector3 pivot)
        {
            var rectTrans = target.GetComponent<RectTransform>();
            if (rectTrans == null)
            {
                Debug.LogError("getLocalPositioByArchor error: dose not contain RectTransform !");
                return Vector3.zero;
            }
            var newPivot = new Vector3(pivot.x - rectTrans.pivot.x, pivot.y - rectTrans.pivot.y, pivot.z);

            return new Vector3(
                rectTrans.localPosition.x + rectTrans.rect.width * newPivot.x,
                rectTrans.localPosition.y + rectTrans.rect.height * newPivot.y,
                rectTrans.localPosition.z);
        }

        static public void setWorldPositionByPivot(GameObject target, Vector3 newPosition, Vector3 pivot)
        {
            var rectTrans = target.GetComponent<RectTransform>();
            if (rectTrans == null)
            {
                Debug.LogError("setLocalPositionByArchor error: target dose not contain RectTransform !");
                return;
            }
            var newPivot = new Vector3(pivot.x - rectTrans.pivot.x, pivot.y - rectTrans.pivot.y, pivot.z);
            var newOffset = new Vector3(rectTrans.rect.width * newPivot.x, rectTrans.rect.height * newPivot.y, 0);
            newOffset = target.transform.TransformPoint(newOffset);

            target.transform.position += new Vector3(
                newPosition.x - newOffset.x,
                newPosition.y - newOffset.y,
                0);
        }

        static public Vector3 getWorldPositioByPivot(GameObject target, Vector3 pivot)
        {
            var rectTrans = target.GetComponent<RectTransform>();
            if (rectTrans == null)
            {
                Debug.LogError("getWorldPositioByPivot error: dose not contain RectTransform !");
                return Vector3.zero;
            }

            var newPivot = new Vector3(pivot.x - rectTrans.pivot.x, pivot.y - rectTrans.pivot.y, pivot.z);
            var t1 = target.transform.TransformPoint(pivot);
            var t2 = target.transform.TransformPoint(new Vector3(rectTrans.rect.width, rectTrans.rect.height));
            var t3 = t2 - t1;
            var t4 = new Vector3(t3.x * newPivot.x, t3.y * newPivot.y, 0);

            var ret = new Vector3(
                rectTrans.position.x + t4.x,
                rectTrans.position.y + t4.y,
                rectTrans.position.z);

            return ret;
        }
    }
}