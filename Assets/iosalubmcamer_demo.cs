using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class iosalubmcamer_demo : MonoBehaviour
{
	public RawImage rawImage;
	public Button btn_photos;
	public Button btn_album;
	public Button btn_camera;
	public Button btn_saveToAlbum;
	public Text txt_saveTip;

	// Use this for initialization
	void Start ()
	{
		if (IOSAlbumCamera.Instance)
		{
			IOSAlbumCamera.Instance.CallBack_PickImage_With_Base64 += callback_PickImage_With_Base64;
			IOSAlbumCamera.Instance.CallBack_ImageSavedToAlbum += callback_imageSavedToAlbum;
		}

		btn_photos.onClick.AddListener (onclick_photos);
		btn_album.onClick.AddListener (onclick_album);
		btn_camera.onClick.AddListener (onclick_camera);
		btn_saveToAlbum.onClick.AddListener (onclick_saveToAlbum);
	}

	void OnDestroy()
	{
		if (IOSAlbumCamera.Instance)
		{
			IOSAlbumCamera.Instance.CallBack_PickImage_With_Base64 -= callback_PickImage_With_Base64;
			IOSAlbumCamera.Instance.CallBack_ImageSavedToAlbum -= callback_imageSavedToAlbum;
		}
	}
	
	void onclick_photos()
	{
		IOSAlbumCamera.iosOpenPhotoLibrary (true);
	}

	void onclick_album()
	{
		IOSAlbumCamera.iosOpenPhotoAlbums (true);
	}

	void onclick_camera()
	{
		IOSAlbumCamera.iosOpenCamera (true);
	}

	void onclick_saveToAlbum()
	{
		string path = Application.persistentDataPath + "/lzhscreenshot.png";
		Debug.Log (path);
		byte[] bytes = (rawImage.texture as Texture2D).EncodeToPNG ();
		System.IO.File.WriteAllBytes (path, bytes);

		IOSAlbumCamera.iosSaveImageToPhotosAlbum (path);

	}

	void callback_PickImage_With_Base64(string base64)
	{
		Texture2D tex = IOSAlbumCamera.Base64StringToTexture2D (base64);
		rawImage.texture = tex;
	}

	void callback_imageSavedToAlbum(string msg)
	{
		txt_saveTip.text = msg;
	}
}
