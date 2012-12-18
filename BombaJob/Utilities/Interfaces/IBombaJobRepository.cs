using System;
using System.Collections.ObjectModel;
using BombaJob.Database.Domain;

namespace BombaJob.Utilities.Interfaces
{
    public interface IBombaJobRepository
    {
        void AddText(Text ent);
        void UpdateText(Text ent);
        void RemoveText(Text ent);
        ObservableCollection<Text> GetTexts();
        Text GetTextById(int textID);

        void AddCategory(Category ent);
        void UpdateCategory(Category ent);
        void RemoveCategory(Category ent);
        ObservableCollection<Category> GetCategories();
        ObservableCollection<Category> GetCategoriesFor(bool humanYn);
        Category GetCategoryById(int entID);

        void AddJobOffer(JobOffer ent);
        void UpdateJobOffer(JobOffer ent);
        void RemoveJobOffer(JobOffer ent);
        ObservableCollection<JobOffer> GetNewestOffers();
        ObservableCollection<JobOffer> GetNewestOffers(int offersPerPage);
        ObservableCollection<JobOffer> GetJobOffers();
        ObservableCollection<JobOffer> GetJobOffers(int offersPerPage);
        ObservableCollection<JobOffer> GetJobOffers(int offersPerPage, int categoryID);
        ObservableCollection<JobOffer> GetPeopleOffers();
        ObservableCollection<JobOffer> GetPeopleOffers(int offersPerPage);
        ObservableCollection<JobOffer> GetPeopleOffers(int offersPerPage, int categoryID);
        JobOffer GetJobOfferById(int entID);
        ObservableCollection<JobOffer> GetByCategoryID(int categoryID);
        ObservableCollection<JobOffer> SearchJobOffers(string searchKeyword);
        int GetJobOffersCount();
        void MarkAsRead(JobOffer ent);
    }
}
