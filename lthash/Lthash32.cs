using System;
using Blake3;
using Lthash.FlatBuffers;

namespace Lthash
{
    public interface ILthash32
    {
        void Add(params byte[][] inputs);
        void Remove(params byte[][] inputs);
        void Update(byte[] oldValue, byte[] newValue);
        void Reset();
        void SetChecksum(byte[] checksum);
        byte[] GetChecksum();
        bool ChecksumEquals(byte[] otherChecksum);
    }
    
    public class Lthash32: ILthash32
    {
        private const int IntSizeInBytes = sizeof(int)/sizeof(byte);
        private const int ChecksumSize = 2048;
        
        private byte[] _checksum = new byte[ChecksumSize];
        private bool _isRemoving = false;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputs"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Add(params byte[][] inputs)
        {
            ApplyInputsToChecksum(inputs);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputs"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Remove(params byte[][] inputs)
        {
            _isRemoving = true;
            ApplyInputsToChecksum(inputs);
            _isRemoving = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        public void Update(byte[] oldValue, byte[] newValue)
        {
            Remove(oldValue);
            Add(newValue);
        }
        
        /// <summary>
        /// 
        /// </summary>
        public void Reset()
        {
            _checksum = Array.Empty<byte>();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="checksum"></param>
        /// <exception cref="ArgumentException"></exception>
        public void SetChecksum(byte[] checksum)
        {
            if (checksum.Length != ChecksumSize)
                throw new ArgumentException(
                    $"{nameof(checksum)} Illegal 'checksum' provided: the checksum must be of {ChecksumSize} bytes");
            DeepCopy(checksum, _checksum);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public byte[] GetChecksum()
        {
            var copy = new byte[ChecksumSize];
            DeepCopy(_checksum, copy);
            return copy;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="otherChecksum"></param>
        /// <returns></returns>
        public bool ChecksumEquals(byte[] otherChecksum)
        {
            var x = _checksum.Length ^ otherChecksum.Length;
            for (var i = 0; i < _checksum.Length && i < otherChecksum.Length; ++i)
            {
                x |= _checksum[i] ^ otherChecksum[i];
            }

            return x == 0;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputs"></param>
        private void ApplyInputsToChecksum(params byte[][] inputs)
        {
            if (inputs == null) return;
            foreach (var input in inputs)
            {
                ApplyInputToChecksum(input);
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        private void ApplyInputToChecksum(byte[] input) {
            var hash = Hasher.Hash(input);
            _checksum = ApplyHashToChecksum(hash.AsSpan().ToArray());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputHash"></param>
        /// <returns></returns>
        private byte[] ApplyHashToChecksum(byte[] inputHash)
        {
            var checksumWrap = new ByteBuffer(new ByteArrayAllocator(_checksum), 0);
            var newHashWrap = new ByteBuffer(new ByteArrayAllocator(inputHash), 0);
            for (var i = 0; i < inputHash.Length; i += IntSizeInBytes)
            {
                var sum = _isRemoving
                    ? checksumWrap.GetInt(i) - newHashWrap.GetInt(i)
                    : checksumWrap.GetInt(i) + newHashWrap.GetInt(i);
                checksumWrap.PutInt(i, sum);
            }

            return checksumWrap.ToArray(0, ChecksumSize);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private static void DeepCopy(byte[] source, byte[] destination)
        {
            if (source.Length != destination.Length)
                throw new ArgumentOutOfRangeException($"{nameof(source)} Bad input arrays in deep copy");
            Array.Copy(source, 0, destination, 0, ChecksumSize);
        }
    }
}