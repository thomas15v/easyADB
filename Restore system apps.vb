Imports System.Windows.Forms
Imports System.IO

Public Class Restore_system_apps
    Private Sub Restore_system_apps_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        Label1.Text = "About Restoring System application" & vbNewLine & "  1. Boot in to recovery and connect device" & vbNewLine & "  2. Press Check connection button" & vbNewLine & "  3. Select the back-up" & vbNewLine & "  4. Press the flash back button"
        Label2.Text = "Connected = False"
        Button2.Enabled = False
        ListBox1.Enabled = False
        Dim count As Integer = 0
        If Directory.Exists("\backup") Then
            Dim max As Integer = My.Computer.FileSystem.GetFiles("backup").Count
            Do While count < max
                ListBox1.Items.Add(My.Computer.FileSystem.GetFileInfo(My.Computer.FileSystem.GetFiles("backup").Item(count)).Name)
                count += 1
            Loop
        End If
        If Device_connected(0) = True Then
            Label2.Text = "Connected = True"
            ListBox1.Enabled = True
        Else
            Label2.Text = ""
            System.Threading.Thread.Sleep(200)
            Label2.Text = "Connected = False"
            ListBox1.Enabled = False
            Button2.Enabled = False
        End If
    End Sub

    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click
        If Device_connected(0) = True Then
            Label2.Text = "Connected = True"
            ListBox1.Enabled = True
        Else
            Label2.Text = ""
            System.Threading.Thread.Sleep(200)
            Label2.Text = "Connected = False"
            ListBox1.Enabled = False
            Button2.Enabled = False
        End If
    End Sub

    Private Sub Button2_Click(sender As System.Object, e As System.EventArgs) Handles Button2.Click
        If Not ListBox1.SelectedItem = Nothing Then
            Form1.adb_command("reboot")
            Form1.adb_command("push """ & Application.StartupPath & "/backup/" & ListBox1.SelectedItem & """ /system/app")
            Form1.adb_command("shell chmod 644 /system/app/" & ListBox1.SelectedItem)
            Form1.adb_command("reboot")
            MsgBox("Done, rebooting...")
            Me.Close()
        End If
    End Sub

    Private Sub ListBox1_Click(sender As Object, e As System.EventArgs) Handles ListBox1.Click
        Button2.Enabled = True
    End Sub
End Class
