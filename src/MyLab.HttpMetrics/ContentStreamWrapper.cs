using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MyLab.HttpMetrics
{
    class ContentStreamWrapper : Stream
    {
        private readonly Stream _origin;

        public long ReadCounter { get; private set; }
        public long WriteCounter { get; private set; }

        public override int ReadTimeout
        {
            get => _origin.ReadTimeout;
            set => _origin.ReadTimeout = value;
        }

        public override int WriteTimeout
        {
            get => _origin.WriteTimeout;
            set => _origin.WriteTimeout = value;
        }

        public override bool CanTimeout => _origin.CanTimeout;


        public override bool CanRead => _origin.CanRead;
        public override bool CanSeek => _origin.CanSeek;
        public override bool CanWrite => _origin.CanWrite;
        public override long Length => _origin.Length;
        public override long Position
        {
            get => _origin.Position;
            set => _origin.Position = value;
        }

        public ContentStreamWrapper(Stream origin)
        {
            _origin = origin;
        }

        public override void Flush()
        {
            _origin.Flush();
        }

        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            return _origin.FlushAsync(cancellationToken);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var read = _origin.Read(buffer, offset, count);
            ReadCounter += read;

            return read;
        }

        public override int Read(Span<byte> buffer)
        {
            var read = _origin.Read(buffer);
            ReadCounter += read;
            return read;
        }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            var read = await  _origin.ReadAsync(buffer, offset, count, cancellationToken);
            ReadCounter += read;
            return read;
        }

        public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = new CancellationToken())
        {
            var read = await _origin.ReadAsync(buffer, cancellationToken);
            ReadCounter += read;
            return read;
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object? state)
        {
            return _origin.BeginRead(buffer, offset, count, callback, state);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _origin.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _origin.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _origin.Write(buffer, offset, count);
            WriteCounter += count;
        }

        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            await _origin.WriteAsync(buffer, offset, count, cancellationToken);
            WriteCounter += count;
        }

        public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = new CancellationToken())
        {
            await _origin.WriteAsync(buffer, cancellationToken);
            WriteCounter += buffer.Length;
        }


        public override void Write(ReadOnlySpan<byte> buffer)
        {
            _origin.Write(buffer);
            WriteCounter += buffer.Length;
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object? state)
        {
            var res = _origin.BeginWrite(buffer, offset, count, callback, state);
            WriteCounter += count;
            return res;
        }

        public override void Close()
        {
            _origin.Close();
        }

        public override void CopyTo(Stream destination, int bufferSize)
        {
            _origin.CopyTo(destination, bufferSize);
        }

        public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
        {
            return _origin.CopyToAsync(destination, bufferSize, cancellationToken);
        }

        public override ValueTask DisposeAsync()
        {
            return _origin.DisposeAsync();
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            var read = _origin.EndRead(asyncResult);
            ReadCounter += read;
            return read;
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            _origin.EndWrite(asyncResult);
        }

        public override object InitializeLifetimeService()
        {
            return _origin.InitializeLifetimeService();
        }

        public override int ReadByte()
        {
            return _origin.ReadByte();
        }

        public override void WriteByte(byte value)
        {
            _origin.WriteByte(value);
            WriteCounter += 1;
        }
    }
}