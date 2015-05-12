using System.Linq;
using EffectFramework.Core.Models.Db;

namespace EffectFramework.Console
{
    public class Program
    {
        public void Main(string[] args)
        {
            using (var db = new ItemDb7Context(""))
            {
                Item NewItem = new Item()
                {
                    DisplayName = "Test",
                    IsDeleted = false
                };

                db.Items.Add(NewItem);
                db.SaveChanges();

                var x = db.Items.Count();

                Item ItemRemove = db.Items.Where(z => z.ItemID == NewItem.ItemID && NewItem.IsDeleted == false).First();

                db.Items.Remove(ItemRemove);
                db.SaveChanges();
            }
            System.Console.In.ReadLine();
        }
    }
}
