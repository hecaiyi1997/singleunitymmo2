using UnityEngine;



namespace UnityMMO
{
    public class MainRole : MonoBehaviour
    {
        public float turnSpeed = 50f;
        void Awake()
        {

        }
        public void setparent(Transform parent)
        {
            this.gameObject.transform.SetParent(parent);
            //this.gameObject.transform.localPosition = new Vector3(0,150,-200);
           // this.gameObject.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
            //this.gameObject.transform.localScale = new Vector3(180, 180, 180);
        }
        public void setpos(Vector3 pos)
        {
            //this.gameObject.transform.SetParent(parent);
            this.gameObject.transform.localPosition = pos;
            //this.gameObject.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
            //this.gameObject.transform.localScale = new Vector3(180, 180, 180);
        }
        void Update()
        {
            //this.transform.Rotate(Vector3.up, turnSpeed * Time.deltaTime); //ÈÆ×ÔÉíXÖáÐý×ª
        }
    }
}

