using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Autumn.Utils
{
	public class ByteArrayComparer : IEqualityComparer<byte[]>
	{
		public bool Equals(byte[] left, byte[] right)
		{
			if (left == null || right == null)
			{
				return left == right;
			}

			return left.SequenceEqual(right);
		}

		public int GetHashCode(byte[] key)
		{
			if (key == null)
				throw new ArgumentNullException(nameof(key));
			return key.Sum(b => b);
		}
	}

	public static class HashManager
	{
		private static Dictionary<byte[],HMACSHA256> _hash_cach = new Dictionary<byte[], HMACSHA256>(new ByteArrayComparer());
		
		public static byte[] Hmacsha256(byte[] keyByte, byte[] messageBytes)
		{
			if (!_hash_cach.TryGetValue(keyByte, out HMACSHA256 hash))
			{
				hash = new HMACSHA256(keyByte);
				_hash_cach.Add(keyByte, hash);
			}
			return hash.ComputeHash(messageBytes);
		}
		
		public static string Hmacsha256(string key, string message)
		{
			var keyBytes = Encoding.UTF8.GetBytes(key);
			var messageBytes = Encoding.UTF8.GetBytes(message);
			
			if (!_hash_cach.TryGetValue(keyBytes, out HMACSHA256 hash))
			{
				hash = new HMACSHA256(keyBytes);
				_hash_cach.Add(keyBytes, hash);
			}
			var bytes = hash.ComputeHash(messageBytes);
			return DataUtils.ByteToHex(bytes);
		}

		public static void ClearCach()
		{
			_hash_cach.Clear();
		}
	}
}