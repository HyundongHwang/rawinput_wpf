<!-- TOC -->

- [구현내용](#구현내용)
- [데모](#데모)
- [종속성](#종속성)
- [원본](#원본)
- [참고자료](#참고자료)

<!-- /TOC -->

# 구현내용
- rawinput 을 이용하여 키보드, 마우스 디바이스 HID인식후 키입력 내용까지 표시
- 특정 입력장치에서 발생시킨 이벤트를 윈도우메시지 이전타임에 캡쳐하여 활용(다른 프로세스로 IPC전달 등...) 할 수 있다.
- 이렇게 캡쳐한 이후에 윈도우메시지 블로킹하여 일반입력장치로는 사용불가하게 만드는 예제는 준비중 ...

# 데모
![](http://s30.postimg.org/yyqomnigx/image.png)

# 종속성
- 닷넷프레임워크 4.0
    - http://ec2-52-78-167-96.ap-northeast-2.compute.amazonaws.com/coconut/dotNetFx40_Full_x86_x64.exe

# 원본
http://www.codeproject.com/Articles/17123/Using-Raw-Input-from-C-to-handle-multiple-keyboard

# 참고자료
https://www.codeproject.com/Articles/716591/Combining-Raw-Input-and-keyboard-Hook-to-selective
http://m.blog.naver.com/ses4737/30123530503
http://egloos.zum.com/javahawk/v/10855658
https://msdn.microsoft.com/ko-kr/library/windows/desktop/ms645600(v=vs.85).aspx
https://msdn.microsoft.com/ko-kr/library/windows/desktop/ms645536(v=vs.85).aspx
https://msdn.microsoft.com/en-us/library/windows/desktop/ms645543(v=vs.85).aspx
https://www.codeproject.com/articles/381673/using-the-rawinput-api-to-process-multitouch-digit

