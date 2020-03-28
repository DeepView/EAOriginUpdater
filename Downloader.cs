using System;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
namespace EAOriginUpdater
{
   /// <summary>
   /// 表示一个可以下载文件的下载器类。
   /// </summary>
   [Serializable]
   [ComVisible(true)]
   public class Downloader
   {
      private string _sourceUri;//文件在互联网上的URL字符串。
      private string _savedPath;//需要保存的文件路径。
      /// <summary>
      /// 构造函数，创建一个包含文件下载源和保存路径的Downloader实例。
      /// </summary>
      /// <param name="sourceUri">文件在互联网上的URL字符串，即文件下载源。</param>
      /// <param name="savedPath">下载的文件的保存路径。</param>
      public Downloader(string sourceUri, string savedPath)
      {
         SourceUri = sourceUri;
         SavedPath = savedPath;
      }
      /// <summary>
      /// 获取或设置当前实例的文件下载源。
      /// </summary>
      public string SourceUri { get => _sourceUri; set => _sourceUri = value; }
      /// <summary>
      /// 获取或设置当前实例的文件保存路径。
      /// </summary>
      public string SavedPath { get => _savedPath; set => _savedPath = value; }
      /// <summary>
      /// 通过HTTP的方式下载指定的文件，其下载源和保存路径由SourceUri属性和SavedPath属性指定。
      /// </summary>
      public void HttpDownload()
      {
         HttpWebRequest request = WebRequest.Create(SourceUri) as HttpWebRequest;
         HttpWebResponse response = request.GetResponse() as HttpWebResponse;
         Stream responseStream = response.GetResponseStream();
         Stream stream = new FileStream(SavedPath, FileMode.Create);
         byte[] bArr = new byte[1024];
         int size = responseStream.Read(bArr, 0, (int)bArr.Length);
         while (size > 0)
         {
            stream.Write(bArr, 0, size);
            size = responseStream.Read(bArr, 0, (int)bArr.Length);
         }
         stream.Close();
         responseStream.Close();
      }
   }
}
