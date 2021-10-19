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
    Button btn_requestImg;

    public Texture2D m_uploadImg;
    private Texture2D m_downloadImg;

    private bool is_connecting = false;

    Image img;

    AudioClip m_soundClip;

    Button btn_sound;

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
                else if (trans.name.CompareTo("btn_requestImg") == 0)
                {
                    btn_requestImg = trans.GetComponent<Button>();
                }
                else if (trans.name.CompareTo("Image") == 0)
                {
                    img = trans.GetComponent<Image>();
                }
                else if (trans.name.CompareTo("btn_sound") == 0)
                {
                    btn_sound = trans.GetComponent<Button>();
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

            btn_requestImg?.onClick.AddListener(() => {

                StartCoroutine(IRequestPNG());
            });

            btn_sound?.onClick.AddListener(() =>
            {
                StartCoroutine(IPlaySound());
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

        UnityWebRequest www = UnityWebRequest.Post("http://localhost:25279/StorageData/PostData", formData);

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

    private IEnumerator IRequestPNG()
    {
        byte[] bs = m_uploadImg.EncodeToJPG();

        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormFileSection("image", bs, "screenshot", "image/jpeg"));

        UnityWebRequest www = UnityWebRequest.Post("http://localhost:25279/StorageData/RequestImage", formData);

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            this.updateResult(www.error);
            yield return null;
        }
        else
        {
            if (www.isDone)
            {
                m_downloadImg = new Texture2D((int)img.rectTransform.rect.width, (int)img.rectTransform.rect.height);

                m_downloadImg.LoadImage(www.downloadHandler.data);

                Sprite sprite = Sprite.Create(m_downloadImg, new Rect(0, 0, m_downloadImg.width, m_downloadImg.height), new Vector2(0.5f, 0.5f));
                img.sprite = sprite;

                Resources.UnloadUnusedAssets();
            }
        }
    }

    private IEnumerator IPlaySound()
    {
        UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("http://localhost:25279/StorageData/GetSounds", AudioType.WAV);

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            this.updateResult(www.error);
            yield return null;
        }
        else
        {
            if (www.isDone)
            {
                m_soundClip = DownloadHandlerAudioClip.GetContent(www);

                GetComponent<AudioSource>().PlayOneShot(m_soundClip);
            }
        }
    }
}
