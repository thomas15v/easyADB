Imports System.Windows.Forms

Public Class Dialog1

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        'Me.Close()
        If CheckBox3.Checked = True Then
            installcomand = "#INSTALLSYSTEM#" & TextBox1.Text
        Else
            Dim SDcard As String
            Dim arguments As String
            If CheckBox2.Checked = True Then
                SDcard = "-s"
            Else
                SDcard = ""
            End If

            If CheckBox1.Checked = True Then
                arguments = "-r"
            Else
                arguments = ""
            End If
            installcomand = "install " & arguments & " " & SDcard & " """ & TextBox1.Text & """"
            Close()
        End If
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click
        APKhandler.ShowDialog()
    End Sub

    Public Sub APKhandler_FileOk(sender As System.Object, e As System.ComponentModel.CancelEventArgs) Handles APKhandler.FileOk
        TextBox1.Text = APKhandler.FileName
        CheckBox3.Enabled = True
        CheckBox2.Enabled = True
        CheckBox1.Enabled = True
    End Sub

    Private Sub Dialog1_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        If APKhandler.FileName = Nothing Then
            TextBox1.ReadOnly = True
            CheckBox3.Enabled = False
            CheckBox2.Enabled = False
            CheckBox1.Enabled = False
        Else
            APKhandler_FileOk(Nothing, Nothing)
        End If
    End Sub

    Private Sub CheckBox3_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles CheckBox3.CheckedChanged
        If CheckBox3.Checked = True Then
            If MsgBox("Warning, Is the selected file a system application?", MsgBoxStyle.YesNo Or MsgBoxStyle.Critical) = MsgBoxResult.Yes Then
                CheckBox1.Enabled = False
                CheckBox2.Enabled = False
            End If
        Else
            CheckBox1.Enabled = True
            CheckBox2.Enabled = True
        End If
    End Sub
End Class
