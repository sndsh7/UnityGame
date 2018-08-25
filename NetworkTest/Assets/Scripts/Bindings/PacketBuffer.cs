using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class PacketBuffer : IDisposable
{
    List<byte> _bufferlist;
    byte[] _readbuffer;
    int _readpos;
    bool _bufferupdate = false;
    public PacketBuffer()
    {
        _bufferlist = new List<byte>();
        _readpos = 0;
    }
    public int GetReadPos()
    {
        return _readpos;
    }
    public byte[] ToArray()
    {
        return _bufferlist.ToArray();
    }
    public int Count()
    {
        return _bufferlist.Count;
    }
    public int Length()
    {
        return Count() - _readpos;
    }
    public void Clear()
    {
        _bufferlist.Clear();
        _readpos = 0;
    }

    // Write data
    public void WriteByte(Byte[] input)
    {
        _bufferlist.AddRange(input);
        _bufferupdate = true;
    }
    public void WriteByte(Byte input)
    {
        _bufferlist.Add(input);
        _bufferupdate = true;
    }
    public void WriteInteger(int input)
    {
        _bufferlist.AddRange(BitConverter.GetBytes(input));
        _bufferupdate = true;
    }
    public void WriteFloat(float input)
    {
        _bufferlist.AddRange(BitConverter.GetBytes(input));
        _bufferupdate = true;
    }
    public void WriteString(String input)
    {
        _bufferlist.AddRange(BitConverter.GetBytes(input.Length));
        _bufferlist.AddRange(Encoding.ASCII.GetBytes(input));
        _bufferupdate = true;
    }


    // Read data
    public int ReadInteger(bool peek = true)
    {
        if (_bufferlist.Count > _readpos)
        {
            if (_bufferupdate)
            {
                _readbuffer = _bufferlist.ToArray();
                _bufferupdate = false;
            }
            int value = BitConverter.ToInt32(_readbuffer, _readpos);
            if (peek & _bufferlist.Count > _readpos)
            {
                _readpos += 4;
            }
            return value;
        }
        else
        {
            throw new Exception("Buffer is past its limit");
        }
    }
    public float ReadFloat(bool peek = true)
    {
        if (_bufferlist.Count > _readpos)
        {
            if (_bufferupdate)
            {
                _readbuffer = _bufferlist.ToArray();
                _bufferupdate = false;
            }
            float value = BitConverter.ToSingle(_readbuffer, _readpos);
            if (peek & _bufferlist.Count > _readpos)
            {
                _readpos += 4;
            }
            return value;
        }
        else
        {
            throw new Exception("Buffer is past its limit");
        }
    }
    public byte ReadByte(bool peek = true)
    {
        if (_bufferlist.Count > _readpos)
        {
            if (_bufferupdate)
            {
                _readbuffer = _bufferlist.ToArray();
                _bufferupdate = false;
            }
            Byte value = _readbuffer[_readpos];
            if (peek & _bufferlist.Count > _readpos)
            {
                _readpos += 4;
            }
            return value;
        }
        else
        {
            throw new Exception("Buffer is past its limit");
        }
    }
    public byte[] ReadBytes(int length, bool peek = true)
    {
        if (_bufferupdate)
        {
            _readbuffer = _bufferlist.ToArray();
            _bufferupdate = false;
        }
        byte[] value = _bufferlist.GetRange(_readpos, length).ToArray();
        if (peek & _bufferlist.Count > _readpos)
        {
            _readpos += length;
        }
        return value;
    }
    public string ReadString(bool peek = true)
    {
        int lenght = ReadInteger(true);
        if (_bufferupdate)
        {
            _readbuffer = _bufferlist.ToArray();
            _bufferupdate = false;
        }
        string value = Encoding.ASCII.GetString(_readbuffer, _readpos, lenght);
        if (peek & _bufferlist.Count > _readpos)
        {
            _readpos += lenght;
        }
        return value;
    }

    //IDisposable
    private bool disposedValue = false;
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _bufferlist.Clear();
            }
            _readpos = 0;
        }
        disposedValue = true;
    }
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}