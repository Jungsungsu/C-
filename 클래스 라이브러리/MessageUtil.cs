using System;
using System.IO;

namespace FUP
{
    public class MessageUtil
    {
        public static void Send(Stream writer, Message msg) // Send() 메소드는 스트림을 통해 메시지를 내보냅니다.
        {
            writer.Write(msg.GetBytes(), 0, msg.GetSize());
        }
        public static Message Receive(Stream reader)
        {
            int totalRecv = 0;
            int sizeToRead = 16;
            byte[] hBuffer = new byte[sizeToRead];

            while (sizeToRead > 0)
            {
                byte[] buffer = new byte[sizeToRead];
                int recv = reader.Read(buffer, 0, sizeToRead);
                if (recv == 0)
                    return null;

                buffer.CopyTo(hBuffer, totalRecv);
                totalRecv += recv;
                sizeToRead -= recv;
            }

            Header header = new Header(hBuffer);

            totalRecv = 0;
            byte[] bBuffer = new byte[header.BODYLEN];
            sizeToRead = (int)header.BODYLEN;

            while (sizeToRead > 0)
            {
                byte[] buffer = new byte[sizeToRead];
                int recv = reader.Read(buffer, 0, sizeToRead);
                if (recv == 0)
                    return null;

                buffer.CopyTo(bBuffer, totalRecv);
                totalRecv += recv;
                sizeToRead -= recv;
            }

            ISerializable body = null;
            switch (header.MSGTYPE) // 헤더의 MSGTYPE 프로퍼티를 통해 어떤 Body 클래스의 생성자를 호출할지를 결정합니다.
            {
                case CONSTANTS.REQ_FILE_SEND:
                    body = new BodyRequest(bBuffer);
                    break;
                case CONSTANTS.REP_FILE_SEND:
                    body = new BodyResponse(bBuffer);
                    break;
                case CONSTANTS.FILE_SEND_DATA:
                    body = new BodyData(bBuffer);
                    break;
                case CONSTANTS.FILE_SEND_RES:
                    body = new BodyResult(bBuffer);
                    break;
                default:
                    throw new Exception(
                        string.Format(
                            "Unknown MSGTYPE : {0}", header.MSGTYPE));
            }

            return new Message() { Header = header, Body = body };
        }
    }
}
