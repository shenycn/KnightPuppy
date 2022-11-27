    using System;
    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/*[System.Serializable]
public class EventVector3 : UnityEvent<Vector3>
{
        
}*/
public class MouseManager : MonoBehaviour
{
    public static MouseManager Instance;//单例模式的变量
    
    private RaycastHit hitInto;
    
    public event Action<Vector3> OnMouseClicked;
    public event Action<GameObject> OnEnemyClicked;

    public Texture2D point, doorway, attack, target, arrow;//鼠标图片
    
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }

        Instance = this;
    }

    private void Update()
    {
        SetCursorTesture();
        MouseControl();
    }
 
    void SetCursorTesture()
    {
        //射线指到鼠标点击的位置
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hitInto))
        {
            //切换鼠标贴图
            switch (hitInto.collider.gameObject.tag)
            {
                case"groud":
                    Cursor.SetCursor(target,new Vector2(16,16),CursorMode.Auto);
                    break;
                case"Enemy":
                    Cursor.SetCursor(attack,new Vector2(16,16),CursorMode.Auto);
                    break;    
            }
            
        }
    }

    void MouseControl()
    {
        if (Input.GetMouseButtonDown(0) && hitInto.collider != null)
        {
            if (hitInto.collider.gameObject.CompareTag("groud")) 
            OnMouseClicked?.Invoke(hitInto.point);
            
            if (hitInto.collider.gameObject.CompareTag("Enemy")) 
            OnEnemyClicked?.Invoke(hitInto.collider.gameObject);//collider传入gameobject
        }
    }
    
}
