using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace EchoServer
{
    class MainApp
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("사용법 : {0} <Bind IP>",
                    Process.GetCurrentProcess().ProcessName);
                return;
            }

            string bindIp = args[0];
            const int bindPort = 5425;
            TcpListener server = null;
            try
            {
                IPEndPoint localAddress = // IPEndPoint는 IP 통신에 필요한 IP주소와 출입구(포트)를 나타냅니다.
                    new IPEndPoint(IPAddress.Parse(bindIp), bindPort);

                server = new TcpListener(localAddress);

                server.Start(); // server 객체는 클라이언트가 TcpClient.Connect()를 호출하여 연결 요청해오기를 기다리기 시작 합니다.

                Console.WriteLine("메아리 서버 시작...");

                while (true)
                {
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("클라이언트 접속 : {0} ",
                        ((IPEndPoint)client.Client.RemoteEndPoint).ToString());

                    NetworkStream stream = client.GetStream();

                    int length;
                    string data = null;
                    byte[] bytes = new byte[256];

                    while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        data = Encoding.Default.GetString(bytes, 0, length);
                        Console.WriteLine(string.Format("수신: {0}", data));

                        byte[] msg = Encoding.Default.GetBytes(data);

                        stream.Write(msg, 0, length);
                        Console.WriteLine(String.Format("송신: {0}", data));
                    }

                    stream.Close();
                    client.Close();
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                server.Stop();
            }

            Console.WriteLine("서버를 종료합니다.");
        }
    }
}

// 폼 디자이너 : 비주얼 스튜디오 IDE의 일부

// 도구상자와 폼 디자이너를 이용해서 UI를 구성하는 방법
// 1. 도구 상자에서 사용할 컨트롤을 골라 마우스 커서를 위치시키고 왼쪽 버튼을 클릭합니다.
// 2. 마우스 커서를 그대로 폼 디자이너 위로 옮긴 뒤 다시 왼쪽 마우스 버튼을 클릭합니다.
// 3. 폼 위에 올려진 컨트롤의 위치 및 크기, 프로퍼티를 수정합니다.

// 네트워크 프로그래밍
// 네트워크 : 그물에서 파생된 단어로, 어떤 물건이나 사람등의 상호 연결되어 있는 체계
// 프로토콜 : 규약, 규칙이라는 뜻의 낱말로, 여기에서는 컴퓨터들이 네트워크를 통해 데이터를 주고받기 위한 "통신규약"을 뜻함.

// - 프로토콜 계층
// 애플리케이션 계층
// 전송 계층
// 인터넷 계층
// 링크 계층

// RFC : 인터넷국제표준화기구에 의해 발행되는 메모를 말합니다.
// 패킷 : 네트워크를 통해 오고 가는 데이터

// 링크 계층 : TCP/IP는 네트워크의 물리적인 구성으로부터 독립적인 프로토콜이다.
// 인터넷 계층 : 패킷을 수신해야 할 상대의 주소를 지정하고, 나가는 패킷에 대해서는 적절한 크기로 분할하며 들어오는 패킷에 대해서는 재조립을 수행
// 전송 계층 : 패킷의 "운송"을 담당하는 프로토콜이 정의되어 있습니다.
// 애플리케이션 계층 : 각 응용 프로그램 나름의 프로토콜이 정의되는 곳
//                예) HTTP : 웹문서를 주고 받는다
//                    FTP  : 파일 교환을 한다
//                    SNMP : 네트워크 관리를 한다.
// IP 주소 : 인터넷에서 사용하는 주소

// 포트 : 패킷이 드나드는 출입구
// 포트 번호 : HTTP나 FTP, Telnet과 같은 표준 프로토콜이 사용하고 있는 포트 번호는 전 세계적으로 합의된 값

// TcpListener와 TcpClient : .NET 프레임워크가 TCP/IP 통신을 위해 제공하는 클래스
// TcpListener 클래스 : 서버 애플리케이션에서 사용되며, 클라이언트의 연결 요청을 기다리는 역할
// TcpClient : 1.서버 애플리케이션과 클라이언트 애플리케이션 양쪽에서 사용
//             2.서버와 클라이언트가 각각 갖고 있고 GetStream()이라는 메소드를 갖고 있어서, 양쪽의 응용 프로그램은 이 메소드가 반환하는 NetworkStream 객체를 통해 데이터를 주고 받습니다.
//             3.데이터를 보낼 때는 NetworkStream.Write()를, 데이터를 읽을 때는 NetworkStream.Read()를 호출합니다.
//             4.데이터를 주고받는 일을 마치고 나서 서버와 클라이언트의 연결을 종료할때는 NetworkStream 객체와 TcpClient 객체 모두의 Close() 메소드를 호출

// - TcpListener와 TcpClient 클래스의 주요 메소드
//      클래스		        메소드                         설명
//   TcpListener           Start()              연결 요청 수신 대기를 시작합니다.
//                         AcceptTcpClient()    클라이언트의 연결 요청을 수락합니다.
//                         Stop()               연결 요청 수신 대기를 종료합니다.
//   TcpClient             Connect()            서버에 연결을 요청합니다.
//                         GetStream()          데이터를 주고받는데 사용하는 매개체 NetworkStream을 가져옵니다.
//                         Close()              연결을 닫습니다.



