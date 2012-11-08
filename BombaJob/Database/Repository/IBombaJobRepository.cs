using System;
using System.Collections.Generic;
using BombaJob.Database.Domain;

namespace BombaJob.Database
{
    public interface IBombaJobRepository
    {
        void AddText(Text ent);
        void UpdateText(Text ent);
        void RemoveText(Text ent);
        ICollection<Text> GetTexts();
        Text GetTextById(int textID);

        void AddCategory(Category ent);
        void UpdateCategory(Category ent);
        void RemoveCategory(Category ent);
        ICollection<Category> GetCategories();
        ICollection<Category> GetCategoriesFor(bool humanYn);
        Category GetCategoryById(int entID);

        void AddJobOffer(JobOffer ent);
        void UpdateJobOffer(JobOffer ent);
        void RemoveJobOffer(JobOffer ent);
        ICollection<JobOffer> GetNewestOffers();
        ICollection<JobOffer> GetNewestOffers(int offersPerPage);
        JobOffer GetJobOfferById(int entID);
        ICollection<JobOffer> GetByCategoryID(int categoryID);
        ICollection<JobOffer> SearchJobOffers(string searchKeyword);
    }
}
