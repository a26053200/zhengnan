package utils;

import java.io.*;

/**
 * @ClassName: FileUtils
 * @Description: 文件操作工具
 * @Author: zhengnan
 * @Date: 2018/5/31 22:42
 */
public class FileUtils
{
    public String GetTextFile(String path)
    {
        try {
            FileReader fr = new FileReader(path);
            BufferedReader br = new BufferedReader(fr);
            StringBuilder sb = new StringBuilder();
            String str = br.readLine();
            while (str != null)
            {
                sb.append(str);
                str = br.readLine();
            }
            br.close();
            fr.close();
            return sb.toString();
        } catch (Exception e) {
            System.out.println(e.toString());
        }
        return null;
    }

    public void SaveTextFile(String path,String content)
    {
        try {
            FileWriter fileWriter = new FileWriter(path);
            BufferedWriter buffWriter = new BufferedWriter(fileWriter);
            buffWriter.write(content, 0, content.length());
            buffWriter.flush();
            buffWriter.close();
        } catch (IOException e) {
            System.out.println(e.toString());
        }
    }
}
