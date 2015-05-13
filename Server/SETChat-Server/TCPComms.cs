/*
 * 
 *****************************************************************
 *	FILE		:	TCPComms.cs
 *	PROJECT		:	PROG2120 - Assignment #6 - SET Chat
 *	PROGRAMMERS	:	Denys Solomonov
 *	                Grigoriy Kozyrev
 *	STUDENT #'s	:	6849806
 *	(respectively   6             
 *	FIRST VER.	:	23 October 2014
 *	DESCRIPTION	:	SET Chat Server
 *
 *****************************************************************
 *
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.IO.Ports;

namespace SETChat_Server
{
    /// <summary>
    /// Class   :   TCPComm
    ///     Purpose :   handling of all networking-based information
    /// Methods :   send_Msg
    ///     Purpose :   sends a message to client
    /// </summary>
    class TCPComms
    {
        Socket handler;
        List<String> msgQuery;
        Mutex mutex;
        public Boolean Connected { get { return handler.Connected; } }

        /// <summary>
        /// Constructor for the object
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="query"></param>
        /// <param name="mut"></param>
        public TCPComms(Socket socket, List<String> query, Mutex mut)
        {
            handler = socket;
            msgQuery = query;
            mutex = mut;
        }

        /// <summary>
        /// Method  :   rec_Msg
        ///     method to be used whenever a message arrives from the client
        /// </summary>
        /// <returns> string that was read from the client</returns>
        private String rec_Msg()
        {
            //buffer for receiving sting
            byte[] buf = new byte[1024];

            //Call connection handler
            int bytesRecv = handler.Receive(buf);

            //Encode a string in ASCII format
            String str = Encoding.ASCII.GetString(buf, 0, bytesRecv);
            //return the encoded string
            return str;
        }

        /// <summary>
        /// Method  :   send_Msg
        ///     Takes a string, appends datestamp to it, and sends it to handler
        /// </summary>
        /// <param name="data"></param>
        public void send_Msg(String data)
        {
            //Convert string to bytes, appending datestamp first
            byte[] msg = Encoding.ASCII.GetBytes(DateTime.Now + " " + data);

            //Send out the message
            handler.Send(msg);
        }

        /// <summary>
        /// Method  :   Run
        ///     Gets a message using rec_Msg method
        ///     Checks received message for emptyness
        ///     and sends it off via the send_Msg method
        /// </summary>
        public void Run()
        {
            String message;

            while( handler.Connected )
            {
                try
                {
                    message = "";
                    if ((message = rec_Msg()).Equals("") == false)
                    {
                        //Lock the thread with a mutex for a critical operation
                        mutex.WaitOne();
                        msgQuery.Add(message);
                        mutex.ReleaseMutex();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception caught \t:\t" + ex.ToString());
                }
            }
        }
    }
}
