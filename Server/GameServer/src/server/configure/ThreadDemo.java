package server.configure;

/**
 * @ClassName: ThreadDemo
 * @Description: TODO
 * @Author: zhengnan
 * @Date: 2018/7/19 23:53
 */
public class ThreadDemo implements Runnable
{
    public void run()
    {
        while (true)
        {
            for (int i = 1; i <= 10; i++)
            {
                System.out.println(i);
                try
                {
                    Thread.sleep(1000);
                }
                catch (InterruptedException e)
                {
                    e.printStackTrace();
                }
            }
        }
    }

    public static void main(String[] args)
    {
        Thread daemonThread = new Thread(new ThreadDemo());
        daemonThread.setName("测试thread");
        // 设置为守护进程
        daemonThread.setDaemon(true);
        daemonThread.start();
        System.out.println("isDaemon = " + daemonThread.isDaemon());
        //Thread t = new Thread(new ThreadDemo());
        //t.start();
    }

}

