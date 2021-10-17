using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class HttpConnect : MonoBehaviour
{
    Button btn_getData;
    Button btn_postData;
    Text txt_result;

    private bool is_connecting = false;

    // Start is called before the first frame update
    void Start()
    {
        GameObject go = GameObject.Find("Canvas");

        if (go != null)
        {
            foreach(Transform trans in go.GetComponentsInChildren<Transform>())
            {
                if (trans.name.CompareTo("txt_result") == 0)
                {
                    txt_result = trans.GetComponent<Text>();
                    this.updateResult("No Data");
                }
                else if (trans.name.CompareTo("btn_get") == 0)
                {
                    btn_getData = trans.GetComponent<Button>();
                }
                else if (trans.name.CompareTo("btn_post") == 0)
                {
                    btn_postData = trans.GetComponent<Button>();
                }
            }

            btn_getData?.onClick.AddListener(() =>
            {
                if (!is_connecting)
                {
                    is_connecting = true;

                    this.updateResult("GET is waiting for reply...");

                    StartCoroutine(IGetData());
                }
            });

            btn_postData?.onClick.AddListener(() =>
            {
                if (!is_connecting)
                {
                    is_connecting = true;

                    this.updateResult("POST is waiting for reply...");

                    StartCoroutine(IPostData());
                }
            });
        }
    }

    private void updateResult(string result)
    {
        txt_result.text = result;
    }

    private IEnumerator IGetData()
    {
        UnityWebRequest www = UnityWebRequest.Get("http://localhost:25279/StorageData/GetData?name=Tommy");
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            is_connecting = false;
            this.updateResult(www.error);
            yield return null;
        }
        else
        {
            is_connecting = false;
            this.updateResult(www.downloadHandler.text);
        }
    }

    private IEnumerator IPostData()
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("Name", "admin"));
        formData.Add(new MultipartFormDataSection("Password", "123"));
        //formData.Add(new MultipartFormDataSection(""));

        UnityWebRequest www = UnityWebRequest.Post("http://localhost:25279/StorageData/PostData", formData);
        //www.SetRequestHeader("Content-Type", "multipart/form-data");

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            is_connecting = false;
            this.updateResult(www.error);
            yield return null;
        }
        else
        {
            is_connecting = false;
            this.updateResult(www.downloadHandler.text);
        }
    }
}
