package utils;

import com.alibaba.fastjson.JSONObject;

//import io.jsonwebtoken.*;

import java.util.Base64;
import java.util.Date;

/**
 * @ClassName: JwtHelper
 * @Description: Java Web Token
 * @Author: zhengnan
 * @Date: 2018/6/8 1:38
 */
public class JwtHelper
{
    //明文Token,用于测试 以后有时间再做加密的

    //Token 密钥
    public final static String tokenSecretKey = "emhlbmduYW50YW5naHVpanVhbnpoZW5neWk==";
    //Token 过期时间
    public final static int expiresSecond = 5 * 60 * 60 * 1000;

    /**
     * @param id
     * @return Token
     */
    public static String createJWT(String id)
    {
        JSONObject json = new JSONObject();
        json.put("id",id);
        json.put("secretKey",tokenSecretKey);
        json.put("ttlMillis",expiresSecond);
        return Base64.getEncoder().encodeToString(BytesUtils.string2Bytes(json.toString()));
    }

    /**
     * @param jwt
     * @return 是否验证通过
     */
    public static boolean parseJWT(String jwt)
    {
        byte[] jsonBytes = Base64.getDecoder().decode(jwt);
        JSONObject json = JSONObject.parseObject(BytesUtils.readString(jsonBytes));
        long ttlMillis = json.getLong("ttlMillis");
        long now = new Date().getTime();
        return now - ttlMillis < expiresSecond;
    }
    //Sample method to validate and read the JWT
    //public static void parseJWT(String jwt, String secretKey)
    //{
        //This line will throw an exception if it is not a signed JWS (as expected)
        //Claims claims = Jwts.parser()
                //.setSigningKey(Base64.getDecoder().decode(secretKey))
                //.parseClaimsJws(jwt).getBody();
        //System.out.println("ID: " + claims.getId());
        //System.out.println("Subject: " + claims.getSubject());
        //System.out.println("Issuer: " + claims.getIssuer());
        //System.out.println("Expiration: " + claims.getExpiration());
    //}


    //Sample method to construct a JWT
    /*
    public static String createJWT(String id, String issuer, String subject, long ttlMillis, String secretKey)
    {
        //The JWT signature algorithm we will be using to sign the token
        SignatureAlgorithm signatureAlgorithm = SignatureAlgorithm.HS256;

        long nowMillis = System.currentTimeMillis();
        Date now = new Date(nowMillis);

        //We will sign our JWT with our ApiKey secret
        byte[] apiKeySecretBytes = Base64.getDecoder().decode(secretKey);
        Key signingKey = new SecretKeySpec(apiKeySecretBytes, signatureAlgorithm.getJcaName());

        //Let's set the JWT Claims
        JwtBuilder builder = Jwts.builder().setId(id)
                .setIssuedAt(now)
                .setSubject(subject)
                .setIssuer(issuer)
                .signWith(signatureAlgorithm, signingKey);

        //if it has been specified, let's add the expiration
        if (ttlMillis >= 0)
        {
            long expMillis = nowMillis + ttlMillis;
            Date exp = new Date(expMillis);
            builder.setExpiration(exp);
        }

        //Builds the JWT and serializes it to a compact, URL-safe string
        return builder.compact();
    }*/

}
