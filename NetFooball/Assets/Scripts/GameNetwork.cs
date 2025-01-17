﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Diagnostics;


public class GameNetwork : MonoBehaviour
{

	// Cliente
	public static string ClientName = "Client";
	// ip del servidor
	public static string SERVER_IP = "localhost";
    // Puerto del servidor
    private readonly int SERVER_PORT = 13000;
	// Data del mensage
	private byte[] bufferDataReceive = new byte[1024];
	// Socket del cliente
	private Socket clientSocket = new Socket(
		AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp
	);

    Boolean empezar = false;
       
    // Update se llama una vez por frame
    void Update()
    {
		LoopConnection();

		if ( clientSocket.Connected)
			LoopSend();

	}
	// Loop para conectar con el servidor
	private void LoopConnection()
	{
		// Si aún no está conectado
		if (!clientSocket.Connected)
		{
			try
			{
				clientSocket.Connect(SERVER_IP, SERVER_PORT);
			}
			catch (SocketException)
			{
				SceneManager.LoadScene(0);
			}
		}
	}
	// Cuando ya está conectado, metodo para enviar datos
	private void LoopSend()
	{
        if(Game.fin) {
            SceneManager.LoadScene(3);
            Process proceso = new Process();
            proceso.StartInfo.FileName = @"C:\Users\estep\OneDrive\Documentos\GitHub\NetworksProject\NetFotball\AppBuild\Udp_Client\dist\udp_client.exe";
            proceso.Start();
            clientSocket.Close();
        } else {

        GameObject p = gameObject.GetComponent<Game>().player1;
        GameObject p2 = gameObject.GetComponent<Game>().player2;
        GameObject b = gameObject.GetComponent<Game>().ball;
        string name = GetComponent<Game>().player1.gameObject.GetComponent<Player>().Name;

        // Recibir y enviar mensaje
        try
        {
            // Limpiar los datos
            Array.Clear(bufferDataReceive, 0, bufferDataReceive.Length);

            // Tamaño del mensaje recibido
            int receiveSize = clientSocket.Receive(bufferDataReceive);
            //Mensaje recibido 
            string dataReceive = Encoding.ASCII.GetString(bufferDataReceive);
            

            if (dataReceive.Contains("NoPlayer2")) {
                empezar = false;
                SceneManager.LoadScene(2);
                clientSocket.Close();
                LoopConnection();
                empezar = false;
            }

            if (dataReceive.Contains("Wait")) {
                b.GetComponent<Ball>().score1 = 0;
                b.GetComponent<Ball>().score2 = 0;
                b.SetActive(false);
                p.SetActive(false);
                p2.SetActive(false);
                gameObject.GetComponent<Game>().time = 60.0f;
                gameObject.GetComponent<Game>().waiting.text = "Esperando jugadores...";
                empezar = false;
            }

            if(dataReceive.Contains("Connect") && !empezar) { 
                gameObject.GetComponent<Game>().waiting.text = "";
                gameObject.GetComponent<Game>().time = 60.0f;
                p.SetActive(true);
                b.SetActive(true);
                p2.SetActive(true);
                empezar = true;
            }
                       
            if(empezar) {
                gameObject.GetComponent<Game>().UpdatePlay(dataReceive);

                String dataSend = name + "|" + p.transform.position.x + "|" + p.transform.position.y + "|" + p.transform.position.z
                + "|" + p.transform.rotation.x + "|" + p.transform.rotation.y + "|" + p.transform.rotation.z + "|" + p.transform.rotation.w
                + "|" + b.transform.position.x + "|" + b.transform.position.y + "|" + b.transform.position.z + "|" + p.GetComponent<Player>().score
                + "|" + gameObject.GetComponent<Game>().time;
                // Data a enviar

                //
                //Debug.Log("Mensage recibido: " + dataReceive);
                // Mensage para enviar

                byte[] bufferDataSend = Encoding.ASCII.GetBytes(dataSend);
                // Enviar mensaje al servidor
                clientSocket.Send(bufferDataSend);

            }
            //String dataSend = name + "|" + p.transform.position.x + "|" + p.transform.position.y + "|" + p.transform.position.z
            //    + "|" + p.transform.rotation.x + "|" + p.transform.rotation.y + "|" + p.transform.rotation.z + "|" + p.transform.rotation.w
            //    + "|" + b.transform.position.x + "|" + b.transform.position.y + "|" + b.transform.position.z + "|" + p.GetComponent<Player>().score
            //    + "|" + gameObject.GetComponent<Game>().time;
            //// Data a enviar



            ////
            ////Debug.Log("Mensage recibido: " + dataReceive);
            //// Mensage para enviar

            //byte[] bufferDataSend = Encoding.ASCII.GetBytes(dataSend);
            //// Enviar mensaje al servidor
            //clientSocket.Send(bufferDataSend);

            //
        }
		catch (SocketException)
		{
			clientSocket.Shutdown(SocketShutdown.Both);
			clientSocket.Close();
			SceneManager.LoadScene(0);

		}
       }
	}
}
