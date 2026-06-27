// Models/FormMode.cs
namespace InventoryAppCloudDb.Models;

public enum FormMode
{
    Initial,   // 剛開啟，尚未選擇任何操作（按鈕初始狀態）
    Creating,  // 新增中，輸入欄位可編輯，顯示儲存/取消
    Viewing,   // 查看中（點選歷史單據），輸入欄位唯讀
}