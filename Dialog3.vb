Imports System.Windows.Forms

Public Class Dialog3

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        device = "-s " & ListBox1.SelectedItem.ToString.Split(vbTab)(0)
        MsgBox(device)
        Form1.adb_command(device & " shell ls -1a --color=never")
        If Form1.commandoutput.Contains("No such file or directory") Then
            If device.Contains("emulator") Then
                MsgBox("Emulators are not supported by busybox")
            Else
                MsgBox("Error busybox is not installed")
            End If
        End If
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
        Form1.Close()
    End Sub

    Private Sub ListBox1_DoubleClick(sender As Object, e As System.EventArgs) Handles ListBox1.DoubleClick
        OK_Button_Click(Nothing, Nothing)
    End Sub

    Private Sub Dialog3_FormClosed(sender As Object, e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
        Form1.Close()
    End Sub
End Class
