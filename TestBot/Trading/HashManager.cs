using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace TradeTest
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

		public static void ClearCach()
		{
			_hash_cach.Clear();
		}
	}
}