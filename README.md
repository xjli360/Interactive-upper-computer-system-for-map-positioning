# Interactive-upper-computer-system-for-map-positioning
C#实现，针对辽宁省机器人大赛设计的上位机定位系统

下位机---实现底端硬件控制与底端硬件信息交互，采用stm32编程，

上位机---将下位机底端数据传输给上位机，在上位机上实现可视化的交互显示，上位机亦可发出指令控制下位机，采用C#编程
关于传输的数据，即使你没有下位机发送位置数据，你也可以根据其语义接口与上位机链接你的数据。

通信实现:wifi\buletooth\com端口


文件中“全局定位3.2为最终版本”，文件“11”中含有以往的全局定位版本，包括3.0，3.1，3.2.

---the global positon of robot 这个文件中存着纯净版本，你可以用来扩展和应用在你的项目上

this repository 不包含下位机程序，下位机程序详见：
https://github.com/VulcanLIU/SWJLocation-V2.0.git
