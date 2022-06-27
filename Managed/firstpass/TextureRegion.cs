using Unity.Collections;

public struct TextureRegion
{
	public int x;

	public int y;

	public int bytesPerPixel;

	public NativeArray<byte> bytes;

	public NativeArray<float> floats;

	public int targetWidth;

	public int targetHeight;

	public int pageWidth;

	public TexturePage page;

	public TextureBuffer buffer;

	public TextureRegion(int x, int y, int width, int height, TexturePage page, TextureBuffer buffer)
	{
		this.x = x;
		this.y = y;
		this.page = page;
		this.buffer = buffer;
		targetWidth = width;
		targetHeight = height;
		pageWidth = page.width;
		bytesPerPixel = TextureUtil.GetBytesPerPixel(page.format);
		bytes = page.texture.GetRawTextureData<byte>();
		floats = page.texture.GetRawTextureData<float>();
	}

	private int GetByteIdx(int x, int y)
	{
		int num = x - this.x;
		int num2 = y - this.y;
		return (num + num2 * pageWidth) * bytesPerPixel;
	}

	public void SetBytes(int x, int y, byte b0)
	{
		int byteIdx = GetByteIdx(x, y);
		bytes[byteIdx] = b0;
	}

	public void SetBytes(int x, int y, byte b0, byte b1)
	{
		int byteIdx = GetByteIdx(x, y);
		bytes[byteIdx] = b0;
		bytes[byteIdx + 1] = b1;
	}

	public void SetBytes(int x, int y, byte b0, byte b1, byte b2)
	{
		int byteIdx = GetByteIdx(x, y);
		bytes[byteIdx] = b0;
		bytes[byteIdx + 1] = b1;
		bytes[byteIdx + 2] = b2;
	}

	public void SetBytes(int x, int y, byte b0, byte b1, byte b2, byte b3)
	{
		int byteIdx = GetByteIdx(x, y);
		bytes[byteIdx] = b0;
		bytes[byteIdx + 1] = b1;
		bytes[byteIdx + 2] = b2;
		bytes[byteIdx + 3] = b3;
	}

	public void SetBytes(int x, int y, float v0)
	{
		int index = GetByteIdx(x, y) / 4;
		floats[index] = v0;
	}

	public void SetBytes(int x, int y, float v0, float v1)
	{
		int num = GetByteIdx(x, y) / 4;
		floats[num] = v0;
		floats[num + 1] = v1;
	}

	public void Unlock()
	{
		buffer.Unlock(this);
	}
}
