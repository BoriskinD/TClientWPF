   M S B A M L   `   `   `  ����   
TClientWPF    TClientWPF.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     :bh+http://schemas.microsoft.com/xaml/behaviors       � WindowStyle$
  None=� � Title$ TClient��$	��450��$	��813�� � WindowStartupLocation$ CenterScreen=� � 
ResizeMode$ NoResize=� � ShowInTaskbar �� *ShowInTaskbar+ � WindowState �� *WindowState+ � 	Resources    CenterButtons �� 
TargetType �� Property	 �� Value
 �� 
TargetType �� CornerRadius �� Triggers P� Property P� Value  LeftBarButtons  CenterTextBlock  HeaderButtons��%
  &	        &	 "    &	 D    &	 �    '_� �    �� " ��.���  � ��	 �#FFE373��  � ��$	 Verdana)���  � ��	 �� "
 ��.R��� $	��210��$��25��$ 18��#��z��._��� $��Center=�$��Center=� P�  � {� .  . ���  � ��	 �#A68400P�  �@J�IsMouseOver . . ���  � ��	 �#FFCB00�� " ��.���  � ��	 �#FF8B40��  � ��$	 Verdana)���  � ��	 �� "
 ��.R��� $	��100��$��25��$ 15��#��z��._��� $��Center=�$��Center=� P�  �@J�IsMouseOver . . ���  � ��	 �#FF6400P�  � {� .  . ���  � ��	 �#BF6830�� " ��.���  � ��
	 �������  � ��$
	 Wrap=���  � ��$	 Center=���  � ��$	 Center=��� " ��.���  � ��
	 ���� ��  � ��$	 35����  � ��	 �� "
 ��.R��� #��z��$ 1��._��� $��Center=�$��Center=� P�  �@J�IsMouseOver . . ���  � ��	 �#BCBEE8�� " _�.���  � ��	 �� .R��� .+��� $	��340��$
��Wrap=����� .��� -    �Name$ Parent�� � ColumnDefinitions �� $	��69* ��� $
��350* � � RowDefinitions �� $	��11* ��� $
��186* �.+�� -   $ TopBar�����#0F4DA8$��2�� �� $	��125 ��� �� $
��Auto ��� $
��Auto �.+��� $��TelegramWatcher��
�������$��Center=�$��Center=�$��16��$��	16,4,18,4q�$��92���� $��-��$��2��#��[ ���� *HideWindowCommand+�� $��X��$��3��#��[ ���� *CloseWindowCommand+� -   $ LeftBar�����#284B7E$��1�� �� $��50 ��� .+��� $��Настройки��$	��100��$��25��#��[ {��� *IsSettingsButtonEnable+���� *SettingsCommand+� -   $ Center�����#052F6D$��1��$��1�� �� $
��240* ��� $
��240* ��� $
��240* � �� $	��66* ��� $	��85* ��� $��2* ��� $
��261* ��� $	��26* �.+��� $��0��$��0��$��3�� �� Orientation$ 
Horizontal=�$��Center=�$��Center=�$��60��$	��586��.+��� $��Ищем текст:��$��0��#��[ $��16��$��76��$
��0,22q��� $��0��$��0��$��Center=�$��	10,20,0,0q�.+��� -   $��Center=� �� 	MaxLength$ 25��$��Center=�$	��200��$��20������ *RegexPattern+ �� Mode$ OneWayToSource=��� -    ��Name$ IsTwoStatement��$'��!Двойное выражение��
�������$��15��$��Center=�$��0,5,0,0q� ^� ShowDuration$ 10000�� ^� ToolTip$� �Позволяет искать 2 фразы одновременно. ВАЖНО: Они должны быть разделены пробелом. Eсли указано больше 2-х фраз, то возьмутся только первые 2. Если фраз меньше двух и при этом стоит галочка, то ничего взято НЕ будет.�� d� 	IsChecked �� *IsDoubleStatementChecked+$ OneWayToSource=��� $��в канале��$��10,0,0,0q�#��[ $��16��$��50���� -    ��Name$ comboBox��$��15,0,0,0q� �� DisplayMemberPath$ Value.Title��$	��200��$��20������ *	ChatsList+ �� SelectedItem �� *SelectedChannelData+�� $��3��$��1��$ Vertical=�$��Center=�$��Center=�.+��� $:��4Проверить историю сообщений��$��1��#��[  $��0,5,0,5q����� *CheckMsgHistoryCommand+{��� *IsCheckMsgHistoryButtonEnable+�� $ 
Horizontal=�.+��� .+��� $��Подключиться��#��[  $��5,5,5,5q����� *StartCommand+{��� *IsConnectButtonEnable+�� -   $ IsAutoreconnect��$,��&Автопереподключение��$��Center=�
�������$
 5000��$	��160��$� �Автоматически переподключаться к серверам Telegram в случае проблем с Интернетом.�� �� *IsAutoreconnect+$ OneWayToSource=�{��� *IsAutoreconnectCheckBoxEnable+�� $��Отключиться��#��[  $��0,0,0,15q����� *StopCommand+{��� *IsDisconnectButtonEnable+�� $��3��$��3��$��Лог��
�������$��5,0,5,0q�.���� -	    ��Name$ loger�� �� 
IsReadOnly . " �� VerticalScrollBarVisibility$ Visible=� �� TextWrapping$
 Wrap=����� *Log+$ OneWay=��� $��4��$��3��
������ $��0,0,-0.333,0q�.@��� .���� $��Статус:��
��������� .���� $��30��$��20������ *
IsOnline+�� �� .���� $��Пользователь:��
��������� .���� $��40��
����������� *User+�� �� .���� $<��6Кол-во пересланных сообщений:��
��������� .���� 
����������� *CountOfGeneralFWDMessages+