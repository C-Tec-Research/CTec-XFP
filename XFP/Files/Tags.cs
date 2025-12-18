namespace Xfp.Files
{
    internal abstract class Tags
    {
        internal const string Comment = "//";
        
        internal const string MemoComments = "Memo Comments";

        internal const string ObjectItem      = "Object Item";
        //internal const string ObjectArray     = "ObjectArray";
        internal const string ArrayItem       = "Array Item";
        internal const string Item            = "Item";
        //internal const string ObjectArrayItem = ObjectArray + " " + Item;

        internal const string EndMemo        = "End Memo";
        internal const string EndObject      = "End Object";
        internal const string EndArray       = "End Array";
        internal const string EndObjectList  = "End ObjectList";
        internal const string EndObjectArray = "End ObjectArray";
        internal const string EndFile        = "End File";

        internal const string CastProtocol    = "stCASTLoop";
        internal const string ApolloProtocol  = "stApolloLoop";
    }
}
