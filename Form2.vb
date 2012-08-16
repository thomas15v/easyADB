Public Class Form2
    Public path As String
    Private Sub Form2_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        ToolStripTextBox1.Text = path
        Form1.Process1.StartInfo.Arguments = "shell"
        Dim stopper As Integer = 0
        Form1.Process1.Start()
        Form1.Process1.StandardInput.WriteLine("cat " & path)
        Form1.Process1.StandardInput.WriteLine("exit ")
        TextBox1.Clear()
        Dim count As Integer = 0
        Do Until Form1.Process1.StandardOutput.EndOfStream
            Application.DoEvents()
            Dim output As String = Form1.Process1.StandardOutput.ReadLine
            If Not output = Nothing And count > 4 Then
                TextBox1.Text = TextBox1.Text & output.Replace("~ # exit", "") & vbNewLine
            End If
            count += 1
        Loop
        Form1.Process1.WaitForExit()
    End Sub

    Private Sub SaveToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles SaveToolStripMenuItem.Click
        SaveFileDialog1.FileName = ToolStripTextBox1.Text.Split("/")(ToolStripTextBox1.Text.Split("/").Length - 1).Replace("""", "")
        SaveFileDialog1.ShowDialog()
    End Sub

    Private Sub SaveFileDialog1_FileOk(sender As System.Object, e As System.ComponentModel.CancelEventArgs) Handles SaveFileDialog1.FileOk
        My.Computer.FileSystem.WriteAllText(SaveFileDialog1.FileName, TextBox1.Text, True)
    End Sub
End Class