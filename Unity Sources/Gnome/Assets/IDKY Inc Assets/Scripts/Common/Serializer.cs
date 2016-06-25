// ----------------------------------------------
// 
//             Tearible Monster Run
// 
//  Copyright © 2013 IDKY
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

namespace Idky
{
	using System;
	using System.IO;
	using System.Runtime.Serialization.Formatters.Binary;
	using UnityEngine;

	public static class Serializer
	{
		public static T Deserialize<T>(byte[] data)
		{
			T obj;

			try
			{
				byte[] buffer = new byte[data.Length];
				using (MemoryStream stream = new MemoryStream(buffer))
				{
					stream.Write(data, 0, data.Length);
					stream.Position = 0;
					BinaryFormatter bf = new BinaryFormatter();
					obj = (T) bf.Deserialize(stream);
				}
			}
			catch (Exception ex)
			{	
				Debug.LogError("Deserialization failed: " + ex.Message);
				throw ex;
			}

			return obj;
		}

		public static byte[] Serialize<T>(T data)
		{
			byte[] serializedData;

			try
			{
				using (MemoryStream stream = new MemoryStream())
				{
					BinaryFormatter bf = new BinaryFormatter();
					bf.Serialize(stream, data);
				
					stream.Position = 0;
					serializedData = new byte[stream.Length];
					stream.Read(serializedData, 0, serializedData.Length);
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("Deserialization failed: " + ex.Message);
				throw ex;
			}
			
			return serializedData;
		}
	}
}
