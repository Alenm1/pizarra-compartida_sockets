using System;
using System.Drawing;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
public class DrawingForm : Form
{
    private Panel drawingPanel;
    private TcpClient client;
    private NetworkStream stream;
    private bool isDrawing = false;
    private bool isDrawingLine = false;
    private Point startLinePoint;
    private Point previousPoint;
    private Color currentColor = Color.Black;
    private ComboBox colorComboBox;
    private Button lineButton;
    public DrawingForm()
    {
        Text = "Client Drawing App";
        Width = 900;
        Height = 700;
        // Panel de dibujo
        drawingPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White
        };
        drawingPanel.MouseDown += new MouseEventHandler(DrawingPanel_MouseDown);
        drawingPanel.MouseMove += new MouseEventHandler(DrawingPanel_MouseMove);
        drawingPanel.MouseUp += new MouseEventHandler(DrawingPanel_MouseUp);
        Controls.Add(drawingPanel);
        // Panel para los controles
        FlowLayoutPanel controlPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 40,
            FlowDirection = FlowDirection.LeftToRight
        };
        // Botón para borrar
        Button clearButton = new Button
        {
            Text = "Borrar",
            Width = 80
        };
        clearButton.Click += ClearButton_Click;
        controlPanel.Controls.Add(clearButton);
        // Botón para trazar una línea recta
        lineButton = new Button
        {
            Text = "Línea Recta",
            Width = 80
        };
        lineButton.Click += LineButton_Click;
        controlPanel.Controls.Add(lineButton);
        // ComboBox para seleccionar el color
        colorComboBox = new ComboBox
        {
            Width = 100
        };
        colorComboBox.Items.AddRange(new object[] { "Negro", "Rojo", "Verde", "Azul" });
        colorComboBox.SelectedIndex = 0;
        colorComboBox.SelectedIndexChanged += ColorComboBox_SelectedIndexChanged;
        controlPanel.Controls.Add(colorComboBox);
        Controls.Add(controlPanel);
        ConnectToServer("192.168.13.72", 12345);
    }
    private void ConnectToServer(string server, int port)
    {
        client = new TcpClient(server, port);
        stream = client.GetStream();
        // Start listening for incoming data from the server
        var listenerThread = new System.Threading.Thread(ListenForData);
        listenerThread.Start();
    }
    private void ListenForData()
    {
        while (true)
        {
            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            if (bytesRead > 0)
            {
                string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                string[] parts = message.Split(',');
                if (parts[0] == "clear")
                {
                    Invoke((MethodInvoker)delegate
                    {
                        drawingPanel.Invalidate();
                    });
                }
                else
                {
                    int startX = int.Parse(parts[0]);
                    int startY = int.Parse(parts[1]);
                    int endX = int.Parse(parts[2]);
                    int endY = int.Parse(parts[3]);
                    Color color = Color.FromName(parts[4]);
                    Invoke((MethodInvoker)delegate
                    {
                        using (Graphics g = drawingPanel.CreateGraphics())
                        {
                            using (Pen pen = new Pen(color))
                            {
                                g.DrawLine(pen, new Point(startX, startY), new Point(endX, endY));
                            }
                        }
                    });
                }
            }
        }
    }
    private void DrawingPanel_MouseDown(object sender, MouseEventArgs e)
    {
        if (isDrawingLine)
        {
            startLinePoint = e.Location;
        }
        else
        {
            isDrawing = true;
            previousPoint = e.Location;
        }
    }
    private void DrawingPanel_MouseMove(object sender, MouseEventArgs e)
    {
        if (isDrawing)
        {
            using (Graphics g = drawingPanel.CreateGraphics())
            {
                using (Pen pen = new Pen(currentColor))
                {
                    g.DrawLine(pen, previousPoint, e.Location);
                }
            }
            SendCoordinates(previousPoint.X, previousPoint.Y, e.X, e.Y, currentColor);
            previousPoint = e.Location;
        }
    }
    private void DrawingPanel_MouseUp(object sender, MouseEventArgs e)
    {
        if (isDrawingLine)
        {
            using (Graphics g = drawingPanel.CreateGraphics())
            {
                using (Pen pen = new Pen(currentColor))
                {
                    g.DrawLine(pen, startLinePoint, e.Location);
                }
            }
            SendCoordinates(startLinePoint.X, startLinePoint.Y, e.X, e.Y, currentColor);
        }
        isDrawing = false; // Ensure isDrawing is set to false regardless of the mode
    }
    private void SendCoordinates(int startX, int startY, int endX, int endY, Color color)
    {
        string message = $"{startX},{startY},{endX},{endY},{color.Name}";
        byte[] data = Encoding.ASCII.GetBytes(message);
        stream.Write(data, 0, data.Length);
    }
    private void ClearButton_Click(object sender, EventArgs e)
    {
        drawingPanel.Invalidate();
        string message = "clear";
        byte[] data = Encoding.ASCII.GetBytes(message);
        stream.Write(data, 0, data.Length);
    }
    private void LineButton_Click(object sender, EventArgs e)
    {
        isDrawingLine = !isDrawingLine;
        lineButton.Text = isDrawingLine ? "Modo Dibujo" : "Línea Recta";
    }
    private void ColorComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        switch (colorComboBox.SelectedItem.ToString())
        {
            case "Negro":
                currentColor = Color.Black;
                break;
            case "Rojo":
                currentColor = Color.Red;
                break;
            case "Verde":
                currentColor = Color.Green;
                break;
            case "Azul":
                currentColor = Color.Blue;
                break;
        }
    }
    [STAThread]
    public static void Main()
    {
        Application.Run(new DrawingForm());
    }
}