   M S B A M L   `   `   `  ���� E  ATClientWPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:TClientWPF.ViewsTClientWPF.Views  #    TClientWPF.Views.SettingsView   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     9i+http://schemas.microsoft.com/xaml/behaviors     0localclr-namespace:TClientWPF.Views       � Title$  Settings��$	��340��$	��350�� � WindowStyle$
 None=� � WindowStartupLocation$ CenterScreen=� � 	Resources    HeaderButtons �� 
TargetType �� Property �� Value �� 
TargetType �� CornerRadius	 �� Triggers
 P� Property P� Value  Desc  EnterFields  BottomButtons��%�  &	        &	 �     &	 X    '�� �    &	 �    �� " ��.���  � ��
 ���� ��  � ��$ 30����  � ��$ 20(���  � �� �� " ��.R��� #��z��$ 1��._��� $��Center=�$��Center=�	 P� 
 �@J�IsMouseOver . . ���  � �� �#BCBEE8�� " ��.���  � ��
 �������  � ��$ 5,5,0,5q���  � ��$ 15(���  � ��$ Center=��� " ��.���  � ��$	 220����  � ��$ 25���� BottomButtons��" ��.���  � ��$	 125����  � ��$ 25����  � ��$ 15(��� " ��.���  � �� �#FFE373��  � ��$ Verdana)���  � �� �� " ��.R��� $	��120��$��25��$ 18��#��z��._��� $��Center=�$��Center=�.��� ���#052F6D � RowDefinitions �� $
��Auto ��� .+�� -    �Name$ TopBar�����#0F4DA8 � ColumnDefinitions �� $
��Auto ��� �� $
��Auto �.+��� -    ��Name$ 
WindowName��
�������$��20(�Настройки�� -    ��Name$ Close��$��X��$��3��#��[  ���� *CloseWindowCommand+�� -    ��Name$ groupBox��$(��"Подключение к Telegram��$��1��$��Center=�$	��280��
�������$	��Top=�$	��300��.���  �� $
��220* ��� $	��60* �.+��� -    ��Name$ 	CenterBar��.+��� -   $ Id��#��[ " ID приложения (Api ID)�� -    ��Name$ IdTbox��#��[ ���� *Api_id+ �� Mode$ TwoWay=��� -   $
 Hash��#��[ (&Хэш приложения (Api Hash)�� -	   $��HashTbox��#��[ ���� *
Api_hash+$ TwoWay=��� -
   $ PhoneNumber��#��[ ,*Номер телефона (Phone number)�� -   $��PhoneNumberTbox��#��[ ���� *Phone_Number+$ TwoWay=��� -   $
 Info��$��18��#��[ $
��Wrap=�$��	31,5,26,5q�)'Получить ID и Hash можно �� $"��https://my.telegram.org/apps?� �� CommandParameter$" https://my.telegram.org/apps��
���  �� �� Command �� *NavigateCommand+.�
здесь�� -   $ 	BottomBar�� �� Orientation$ 
Horizontal=�$��1��.+��� -   $ Export��$ ��Экспорт в файл��$��15,5,0,0q�#��[ ���� *SaveCommand+�� -   $ Import��$"��Импорт из файла��$��15,5,0,0q�#��[ ���� *OpenCommand+