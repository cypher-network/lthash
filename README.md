# lthash
A C# implementation of LtHASH (Lattice Hash), a homomorphic hashing algorithm based on lattice cryptography introduced by Mihir Bellare and Daniele Micciancio in [this paper](https://cseweb.ucsd.edu/~mihir/papers/inchash.pdf). This is a simplified C# porting of the [Facebook Folly C++ Library](https://github.com/facebook/folly/tree/master/folly/experimental/crypto) implementation of the algorithm introduced [here](https://code.fb.com/security/homomorphic-hashing/).

## Homomorphic Hashing
A homomorphic hash can simplistically be defined as a hash function such that one can compute the hash of a composite block from the hashes of the individual blocks or rather being *f1* and *f2* two hash functions and *op1*, *op2* two operations it is true that:

```math
f1(a op1 b) = f2(a) op2 f2(b)
```

One of the main building blocks of a homomorphic hashing function is therefore an underlying hash function (our *f2*).
This project depends on [this](https://github.com/xoofx/Blake3.NET) C# implementation of the [BLAKE3](https://github.com/BLAKE3-team/BLAKE3) cryptographic hash function.

## Example
```C#
var ltHash = new Lthash32();

// Create an initial checksum of two inputs
ltHash.Add(System.Text.Encoding.UTF8.GetBytes("apple"), System.Text.Encoding.UTF8.GetBytes("orange"));
var checksum = ltHash.GetChecksum();

// Remove the hash of "apple" from the checksum and check
// if the two checksums are equals
ltHash.Remove(System.Text.Encoding.UTF8.GetBytes("apple"));
var isEqual = ltHash.ChecksumEquals(checksum);

// Update the hash of "orange" with the new value "apple"
// and check if the two checksums are equals
ltHash.Update(System.Text.Encoding.UTF8.GetBytes("orange"), System.Text.Encoding.UTF8.GetBytes("apple"));
isEqual = ltHash.ChecksumEquals(checksum);

// Adding again the missing "orange" and check if the
// checksum is equal to the initial checksum
ltHash.Add(System.Text.Encoding.UTF8.GetBytes("orange"));
isEqual = ltHash.ChecksumEquals(checksum);
```