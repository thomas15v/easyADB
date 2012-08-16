Imports easyADB.Form1
Imports System.IO
Module Module1
    Public installcomand As String
    Public device As String
    Function Device_connected(fuction As Integer) As Boolean
        Device_connected = False
        Form1.Process3.StartInfo.Arguments = "start-server"
        Form1.Process3.Start()
        Form1.Process1.StartInfo.Arguments = "devices"
        Form1.Process1.Start()
        Dialog3.ListBox1.Items.Clear()
        Do Until Form1.Process1.StandardOutput.EndOfStream
            Application.DoEvents()
            Dim output As String = Form1.Process1.StandardOutput.ReadLine
            If Not output = Nothing Then
                If Not output.Contains("List") Then
                    Dialog3.ListBox1.Items.Add(output)
                End If
            End If
        Loop
        If Dialog3.ListBox1.Items.Count > 1 And fuction = 1 Then
            Dialog3.ShowDialog()
        End If
        If Dialog3.ListBox1.Items.Count = 0 Then
            Device_connected = False
        Else
            Device_connected = True
        End If
        Form1.Process1.WaitForExit()
    End Function
End Module
