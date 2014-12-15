using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hi.UrlRewrite
{
    public enum Tokens
    {
        ALL_HTTP,
        ALL_RAW,
        APPL_MD_PATH,
        APPL_PHYSICAL_PATH,
        CERT_COOKIE,
        CERT_FLAGS,
        CERT_ISSUER,
        CERT_KEYSIZE,
        CERT_SECRETKEYSIZE,
        CERT_SERIALNUMBER,
        CERT_SERVER_ISSUER,
        CERT_SUBJECT,
        CONTENT_LENGTH,
        CONTENT_TYPE,
        DOCUMENT_ROOT,
        GATEWAY_INTERFACE,
        HTTP_ACCEPT_ENCODING,
        HTTP_ACCEPT_LANGUAGE,
        HTTP_ACCEPT,
        HTTP_CONNECTION,
        HTTP_CONTENT_LENGTH,
        HTTP_HOST,
        HTTP_IF_MODIFIED_SINCE,
        HTTP_REFERRER,
        HTTP_UA_CPU,
        HTTP_USER_AGENT,
        HTTPS_KEYSIZE,
        HTTPS_SECRETKEYSIZE,
        HTTPS_SERVER_ISSUER,
        HTTPS_SERVER_SUBJECT,
        HTTPS,
        INSTANCE_ID,
        INSTANCE_META_PATH,
        LOCAL_ADDR,
        PATH_INFO,
        PATH_TRANSLATED,
        QUERY_STRING,
        REMOTE_ADDR,
        REMOTE_HOST,
        REMOTE_PORT,
        REMOTE_USER,
        REQUEST_FILENAME,
        REQUEST_METHOD,
        REQUEST_URI,
        SCRIPT_FILENAME,
        SERVER_ADDR,
        SERVER_NAME,
        SERVER_PORT_SECURE,
        SERVER_PORT,
        SERVER_PROTOCOL,
        UNENCODED_URL,
        URL
    }
}