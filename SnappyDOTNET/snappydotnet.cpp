#include "snappy.h"
#include "snappy-internal.h"
#include "snappy-sinksource.h"


namespace SnappyDOTNET{
	using namespace System;
	using namespace System::Runtime::InteropServices;
	using namespace System::Collections;
	using namespace System::IO;


	enum SnappyStatus
	{
		Ok = 0,
		InvalidInput = 1,
		BufferTooSmall = 2
	};

	class CharArrayHelper{
	private:
		char* _array;
	public:
		CharArrayHelper(size_t size){
			_array = new char[size];
		}
		char* Get(){

			return _array;
		}

		~CharArrayHelper(){
			delete[] _array;
		}
	};

	public ref class CompressTool{
	public:

		static array<Byte>^ Compress(array<Byte>^  data){

			if (data == nullptr)
				throw gcnew ArgumentNullException();

			size_t size = data->Length;
			size_t  newSize = snappy::MaxCompressedLength(size);
			
			CharArrayHelper compressedHelper(newSize);
			char* compressed = compressedHelper.Get();
			
			CharArrayHelper tmpHelper(size);
			char* tmp = tmpHelper.Get();
			IntPtr ptr = (IntPtr)tmp;
			Marshal::Copy(data, 0, ptr, size);
			//delete data;

			snappy::RawCompress(tmp, size, compressed, &newSize);


			array<Byte>^ rtn = gcnew array<Byte>(newSize);
			IntPtr sPtr = (IntPtr)compressed;

			Marshal::Copy(sPtr, rtn, 0, newSize);

			return rtn;

		}

		static array<Byte>^ Uncompress(array<Byte>^ compressed){

			if (compressed == nullptr)
				throw gcnew ArgumentNullException();

			size_t length = compressed->Length;
			CharArrayHelper tmpHelper(length);
			char* tmp = tmpHelper.Get();

			IntPtr ptr = (IntPtr)tmp;
			Marshal::Copy(compressed, 0, ptr, length);

			//delete compressed;
			size_t rawSize = 0;

			snappy::GetUncompressedLength(tmp, length, &rawSize);
			
			CharArrayHelper dataHelper(rawSize);
			char*  data = dataHelper.Get();

			snappy::RawUncompress(tmp, length, data);


			array<Byte>^ rtn = gcnew array<Byte>(rawSize);

			IntPtr sPtr = (IntPtr)data;

			Marshal::Copy(sPtr, rtn, 0, rawSize);

			return rtn;

		}

		static int GetMaxCompressedLength(int inLength)
		{
			return snappy::MaxCompressedLength(inLength);
		}

		static int GetUncompressedLength(array<Byte>^ input, int offset, int length)//(Byte* input, size_t inLength, size_t* outLength)
		{

			if (input == nullptr )
				throw gcnew ArgumentNullException();
			if (offset < 0 || length < 0 || offset + length > input->Length)
				throw gcnew ArgumentOutOfRangeException("Selected range is outside the bounds of the input array");
			if (length == 0)
				throw gcnew System::IO::IOException("Compressed block cannot be empty");

			CharArrayHelper dataHelper(length);
			char* data = dataHelper.Get();
			IntPtr ptr = (IntPtr)data;
			Marshal::Copy(input, offset, ptr, length);

			size_t result=0;
			snappy::GetUncompressedLength(data, (size_t)length, &result);
			//delete input;

			return (int)result;

		}


		static int Compress(array<Byte>^ input, int offset, int length,
			array<Byte>^ output, int outOffset)
		{
			if (input == nullptr || output == nullptr)
				throw gcnew ArgumentNullException();
			if (offset < 0 || length < 0 || offset + length > input->Length)
				throw gcnew ArgumentOutOfRangeException("Selected range is outside the bounds of the input array");
			if (outOffset < 0 || outOffset >= output->Length)
				throw gcnew ArgumentOutOfRangeException("Output offset is outside the bounds of the output array");
			size_t  newSize = snappy::MaxCompressedLength(length);

			CharArrayHelper compressedHelper(newSize);
			char* compressed = compressedHelper.Get();

			CharArrayHelper tmpHelper(length);
			char* tmp = tmpHelper.Get();
			IntPtr ptr = (IntPtr)tmp;
			Marshal::Copy(input, offset, ptr, length);

			//delete input;

			snappy::RawCompress(tmp, length, compressed, &newSize);

			IntPtr sPtr = (IntPtr)compressed;

			Marshal::Copy(sPtr, output, outOffset, newSize);

			return (int)newSize;
		
		}

		static int Uncompress(array<Byte>^ input, int offset, int length,
			array<Byte>^ output, int outOffset)
		{
			if (input == nullptr || output == nullptr)
				throw gcnew ArgumentNullException();
			if (offset < 0 || length < 0 || offset + length > input->Length)
				throw gcnew ArgumentOutOfRangeException("Selected range is outside the bounds of the input array");
			if (length == 0)
				throw gcnew System::IO::IOException("Compressed block cannot be empty");
			if (outOffset < 0 || outOffset > output->Length)
				throw gcnew ArgumentOutOfRangeException("Output offset is outside the bounds of the output array");

			CharArrayHelper tmpHelper(length);
			char* tmp = tmpHelper.Get();
			IntPtr ptr = (IntPtr)tmp;
			Marshal::Copy(input, offset, ptr, length);

			size_t rawSize=0;

			snappy::GetUncompressedLength(tmp,length,&rawSize);

			CharArrayHelper rawHelper(rawSize);
			char* raw = rawHelper.Get();

			snappy::RawUncompress(tmp, length, raw);

			IntPtr sPtr = (IntPtr)raw;

			Marshal::Copy(sPtr, output, outOffset, (int)rawSize);
			
			return (int)rawSize;

		}


	};

}

