﻿Imports System.Text.RegularExpressions
Imports MySqlConnector

Public Class LoginForm
    Dim ConnectionString As String = "server=localhost;port=3306;user=root;password=root;database=salesync"
    Dim conn As New MySqlConnection(ConnectionString)

    Private Sub Label1_Click(sender As Object, e As EventArgs) Handles LoginPasswordTextBox.Click
    End Sub

    Private Sub LoginButton_Click(sender As Object, e As EventArgs) Handles LoginButton.Click
        If String.IsNullOrEmpty(EmailTextBox.Text) OrElse String.IsNullOrEmpty(PasswordTextBox.Text) Then
            MessageBox.Show("Please enter your email and password", " ", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        Dim emailRegex As New Regex("^[^@\s]+@[^@\s]+\.[^@\s]+$")
        If emailRegex.IsMatch(EmailTextBox.Text) Then

            Try
                conn.Open()
                Dim email As String = EmailTextBox.Text
                Dim password As String = PasswordTextBox.Text
                Dim salt As String = password & email


                Dim selectQuery As String = "SELECT password, role, name FROM staff WHERE email = @email "


                Using command As New MySqlCommand(selectQuery, conn)
                    command.Parameters.AddWithValue("@email", email)

                    Using reader As MySqlDataReader = command.ExecuteReader()
                        If reader.HasRows Then
                            reader.Read()
                            Dim storedHash As String = reader.GetString(0)
                            Dim role As String = reader.GetString(1)
                            Dim username = reader.GetString(2)

                            Dim passwordMatches As Boolean = VerifyPassword(salt, storedHash)
                            

                            If passwordMatches Then
                                ' MessageBox.Show("Login successful!")
                                EmailTextBox.Clear()
                                PasswordTextBox.Clear()
                                loggedInUser = username
                                Me.Hide()
                                Dim adminDashboard As New AdminDashboard()
                                Dim tellerDashboard As New TellerDashboard()
                                If role = "ADMIN" Then
                                    adminDashboard.Show()
                                Else
                                    tellerDashboard.Show()
                                End If

                            Else
                                    MessageBox.Show("Invalid email or password", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error)
                            End If
                        Else
                            MessageBox.Show("No user found")
                        End If
                    End Using
                End Using

            Catch ex As Exception
                MessageBox.Show("Error logging in: " & ex.Message)
            Finally
                conn.Close()

            End Try
        Else
            MessageBox.Show("Please enter a valid email address", "Invalid Email", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

    End Sub

    Private Sub EmailTextBox_TextChanged(sender As Object, e As EventArgs) Handles EmailTextBox.TextChanged

    End Sub

    Private Sub PasswordTextBox_TextChanged(sender As Object, e As EventArgs) Handles PasswordTextBox.TextChanged

    End Sub

    Private Sub LoginForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub LoginEmailTextBox_Click(sender As Object, e As EventArgs) Handles LoginEmailTextBox.Click

    End Sub
End Class