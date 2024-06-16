using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;

namespace SMTP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<AttachmentModel> attachments = new();
        public MainWindow()
        {
            InitializeComponent();
        }


        private void attachButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog fileDialog = new() { Multiselect = true };
                fileDialog.ShowDialog();

                foreach (var filename in fileDialog.FileNames)
                {
                    if (attachments.All(a => a.Name != filename))
                    {
                        var fileInfo = new FileInfo(filename);
                        var attachment = new AttachmentModel
                        {
                            Name = filename,
                            Size = fileInfo.Length / 1024
                        };
                        attachments.Add(attachment);

                        attachmentListView.Items.Add(attachment);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void sendButton_Click(object sender, RoutedEventArgs e)
        {
            MailAddress from = new(gmailTextBox.Text);
            MailAddress to = new(recepientEmailTextBox.Text);
            MailMessage message = new(from, to)
            {
                Subject = subjectTextBox.Text,
                Body = messageTextBox.Text,
                IsBodyHtml = true,
            };

            foreach (var attachment in attachments)
                message.Attachments.Add(new Attachment(attachment.Name));

            SmtpClient smtpClient = new(serverAddressTextBox.Text, Convert.ToInt32(serverPortTextBox.Text))
            {
                Credentials = new NetworkCredential(gmailTextBox.Text, passwordBlock.Password),
                EnableSsl = true
            };

            try
            {
                smtpClient.Send(message);

                MessageBox.Show("Letter was send", "Success", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            var itemsToRemove = attachmentListView.SelectedItems.Cast<AttachmentModel>().ToList();

            foreach (var attachmentToRemove in itemsToRemove)
                attachments.Remove(attachmentToRemove);

            attachmentListView.Items.Clear();
            foreach (var attachment in attachments)
                attachmentListView.Items.Add(attachment);
        }
    }
    public class AttachmentModel
    {
        public string Name { get; set; }
        public long Size { get; set; }
    }

}
