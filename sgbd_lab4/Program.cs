using System;
using System.Data.SqlClient;
using System.Threading;

namespace SGBD_lab4
{
    class Program
    {
        static int NrIncercari = 10;  // threadul esuat isi va relua executia de 10 ori pana cand tranzactia este considerata fara succes
        static object object1 = new object();
        static object object2 = new object();
        static SqlConnection connection;
        static SqlCommand command;
        static SqlTransaction transaction;

        static void Main(string[] args)
        {
            connection = new SqlConnection("Data Source=DESKTOP-34MD1UU; Initial Catalog=LoveRelationship;Integrated Security=True;");
            connection.Open();
            command = connection.CreateCommand();
            transaction = connection.BeginTransaction();

            Thread t1 = new Thread(Procedure);
            t1.Name = "first";
            Thread t2 = new Thread(Procedure);
            t2.Name = "second";
            t1.Start();
            t2.Start();

            // cand executia threadurilor s-a incheiat, fac commit tranzactiei si inchid conexiunea
            if (t1.ThreadState != ThreadState.Running && t2.ThreadState != ThreadState.Running)
            {
                transaction.Commit();
                connection.Close();
            }
               
        }

        static void Procedure()
        {
            
            for (int i = 0; i < NrIncercari; i++)
            {
                try
                {
                    if (Thread.CurrentThread.Name == "first")   // sunt in primul thread
                    {
                        lock (object1)
                        {
                            Console.WriteLine("first thread acquired a lock on object1");
                            Thread.Sleep(1000);   // astept o secunda pt a ma asigura ca thread-ul al doilea face lock pe object2

                            bool lockTaken = false;
                            Monitor.TryEnter(object2, 2000, ref lockTaken);   // dupa 2 secunde, daca primul thread nu a facut lock pe object2, arunc exceptie
                            if (!lockTaken) throw new TimeoutException();

                            {
                                Console.WriteLine("first thread acquired a lock on object2");
                                command.Connection = connection;
                                command.Transaction = transaction;
                                Console.WriteLine("Transaction started for thread {0}", Thread.CurrentThread.Name);
                                command.CommandText = String.Format("Insert into Pirati VALUES ('Jack', 'Caraibe')");
                                command.ExecuteNonQuery();
                                Console.WriteLine("Commited transaction for thread {0}", Thread.CurrentThread.Name);
                                i = NrIncercari;
                            }

                        }
                    }

                    if (Thread.CurrentThread.Name == "second")   // sunt in al doilea thread
                    {
                        lock (object2)
                        {
                            Console.WriteLine("second thread acquired a lock on object2");
                            Thread.Sleep(1000);
                            lock (object1)
                            { 
                                Console.WriteLine("second thread acquired a lock on object1");
                                command.Connection = connection;
                                command.Transaction = transaction;
                                Console.WriteLine("Transaction started for thread {0}", Thread.CurrentThread.Name);
                                command.CommandText = String.Format("Insert into Pirati VALUES ('Jack', 'Caraibe')");
                                command.ExecuteNonQuery();
                                Console.WriteLine("Commited transaction for thread {0}", Thread.CurrentThread.Name);
                                i = NrIncercari;
                            }
                            
                        }
                    }
                    
                }
                catch (TimeoutException ex)
                {
                 transaction.Rollback();

                    /*
                       folosirea proprietatii Thread.CurrentThread.Name este oarecum triviala avand in vedere ca am "hardcodat" faptul ca 
                       threadul esuat (victima deadlockului) va fi mereu primul thread, insa am folosit-o peste tot pt genericitate
                     */

                    if (i == NrIncercari - 1)  // aceasta a fost ultima incercare; tranzactia nu a fost efectuata si este, asadar, "aborted"
                    {
                        Console.WriteLine("Transaction aborted for thread {0}", Thread.CurrentThread.Name);
                    }
                    else
                    {
                        Console.WriteLine("Exception : {0}", ex.Message);
                        Console.WriteLine("Transaction rolledback for thread {0}", Thread.CurrentThread.Name);
                    }

                }
            }
        }
    }
}