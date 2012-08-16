Imports System.Windows.Forms

Public Class Permission
    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub Timer1_Tick(sender As System.Object, e As System.EventArgs) Handles Timer1.Tick
        Dim chmod As Integer = 0
        If CheckBox1.Checked Then
            chmod += 400
        End If
        If CheckBox2.Checked Then
            chmod += 200
        End If
        If CheckBox3.Checked Then
            chmod += 100
        End If
        If CheckBox6.Checked Then
            chmod += 40
        End If
        If CheckBox5.Checked Then
            chmod += 20
        End If
        If CheckBox4.Checked Then
            chmod += 10
        End If
        If CheckBox9.Checked Then
            chmod += 4
        End If
        If CheckBox8.Checked Then
            chmod += 2
        End If
        If CheckBox7.Checked Then
            chmod += 1
        End If
        Label10.Text = chmod
    End Sub

    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs)
        OpenFileDialog1.ShowDialog()
    End Sub
End Class
