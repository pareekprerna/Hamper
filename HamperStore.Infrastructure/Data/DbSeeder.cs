using HamperStore.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace HamperStore.Infrastructure.Data
{
    public static class DbSeeder
    {
        public static void Seed(AppDbContext context)
        {
            context.Database.Migrate();

            if (context.Cities.Any() || context.Categories.Any() || context.Hampers.Any())
                return; // already seeded

            // ---- Cities ----
            var jaipur = new City { Name = "Jaipur", State = "Rajasthan", IsActive = true, DeliveryFee = 0m, DeliveryDays = 1 };
            var pune = new City { Name = "Pune", State = "Maharashtra", IsActive = true, DeliveryFee = 99m, DeliveryDays = 3 };
            var kota = new City { Name = "Kota", State = "Rajasthan", IsActive = true, DeliveryFee = 49m, DeliveryDays = 2 };
            context.Cities.AddRange(jaipur, pune, kota);

            // ---- Categories ----
            var festival = new Category { Name = "Festival Hampers", Slug = "festival-hampers", Description = "Hampers curated for festive celebrations." };
            var rakhi = new Category { Name = "Rakhi Hampers", Slug = "rakhi-hampers", Description = "Hampers celebrating the bond of Raksha Bandhan." };
            var birthday = new Category { Name = "Birthday Hampers", Slug = "birthday-hampers", Description = "Fun and memorable birthday party return favours." };
            var custom = new Category { Name = "Custom Hampers", Slug = "custom-hampers", Description = "Fully personalized hampers built to your requirements." };
            context.Categories.AddRange(festival, rakhi, birthday, custom);

            context.SaveChanges(); // save so Ids are generated for FK references below

            // ---- Hampers: Rakhi Collection (from "The Hamperly" Rakhi hamper poster) ----
            var tumblerHamper = new Hamper
            {
                Name = "Tumbler Hamper",
                Slug = "tumbler-hamper",
                Description = "Personalized tumbler duo hamper, perfect for siblings celebrating Rakhi.",
                BasePrice = 499m,
                CategoryId = rakhi.Id,
                IsCustomizable = true,
                IsActive = true
            };
            var microHamper = new Hamper
            {
                Name = "Micro Hamper",
                Slug = "micro-hamper",
                Description = "A compact, budget-friendly Rakhi gift box with chocolates and treats.",
                BasePrice = 199m,
                CategoryId = rakhi.Id,
                IsCustomizable = false,
                IsActive = true
            };
            var chocolateBouquet = new Hamper
            {
                Name = "Chocolate Bouquet",
                Slug = "chocolate-bouquet",
                Description = "A beautifully arranged bouquet made entirely of chocolates.",
                BasePrice = 349m,
                CategoryId = rakhi.Id,
                IsCustomizable = true,
                IsActive = true
            };
            var kinderjoyBouquet = new Hamper
            {
                Name = "Kinderjoy Bouquet",
                Slug = "kinderjoy-bouquet",
                Description = "A playful bouquet made of Kinderjoy eggs, great for younger siblings.",
                BasePrice = 449m,
                CategoryId = rakhi.Id,
                IsCustomizable = false,
                IsActive = true
            };
            var lollypopHamper = new Hamper
            {
                Name = "Lollypop Hamper",
                Slug = "lollypop-hamper",
                Description = "A colorful lollipop bunch wrapped with a decorative bow.",
                BasePrice = 249m,
                CategoryId = rakhi.Id,
                IsCustomizable = false,
                IsActive = true
            };
            var miniHamper = new Hamper
            {
                Name = "Mini Hamper",
                Slug = "mini-hamper",
                Description = "A mid-sized Rakhi hamper packed with chips, chocolates and snacks.",
                BasePrice = 599m,
                CategoryId = rakhi.Id,
                IsCustomizable = true,
                IsActive = true
            };
            var smileHamper = new Hamper
            {
                Name = "Smile Hamper",
                Slug = "smile-hamper",
                Description = "A sunflower-shaped balloon hamper filled with photos and treats.",
                BasePrice = 399m,
                CategoryId = rakhi.Id,
                IsCustomizable = true,
                IsActive = true
            };
            var shinchanHamper = new Hamper
            {
                Name = "Shinchan Hamper",
                Slug = "shinchan-hamper",
                Description = "A quirky Shinchan-themed hamper loaded with snacks and toys.",
                BasePrice = 549m,
                CategoryId = rakhi.Id,
                IsCustomizable = false,
                IsActive = true
            };

            // ---- Hampers: Birthday Return Favours (from "The Hamperly" Birthday poster) ----
            var animalPopperFavour = new Hamper
            {
                Name = "Animal Popper Favour",
                Slug = "animal-popper-favour",
                Description = "Cute animal-shaped candy poppers, a favourite kids' party return gift.",
                BasePrice = 19m,
                CategoryId = birthday.Id,
                IsCustomizable = false,
                IsActive = true
            };
            var catFaceLollipopFavour = new Hamper
            {
                Name = "Cat Face Lollipop Favour",
                Slug = "cat-face-lollipop-favour",
                Description = "Adorable cat-face lollipop toppers for birthday return favours.",
                BasePrice = 19m,
                CategoryId = birthday.Id,
                IsCustomizable = false,
                IsActive = true
            };
            var teddyLollipopBag = new Hamper
            {
                Name = "Teddy Lollipop Bag",
                Slug = "teddy-lollipop-bag",
                Description = "Adorable teddy-shaped paper bag with a heart lollipop attached.",
                BasePrice = 35m,
                CategoryId = birthday.Id,
                IsCustomizable = false,
                IsActive = true
            };
            var heartChocolateWreath = new Hamper
            {
                Name = "Heart Chocolate Wreath",
                Slug = "heart-chocolate-wreath",
                Description = "A heart-shaped wreath decorated with mini chocolates.",
                BasePrice = 129m,
                CategoryId = birthday.Id,
                IsCustomizable = false,
                IsActive = true
            };
            var snailLollipopFavour = new Hamper
            {
                Name = "Snail Lollipop Favour",
                Slug = "snail-lollipop-favour",
                Description = "Colorful snail-shaped lollipop toppers, fun for kids' parties.",
                BasePrice = 25m,
                CategoryId = birthday.Id,
                IsCustomizable = false,
                IsActive = true
            };
            var starChocolateFavour = new Hamper
            {
                Name = "Star Chocolate Favour",
                Slug = "star-chocolate-favour",
                Description = "Star-topped chocolate favours wrapped in gold foil.",
                BasePrice = 29m,
                CategoryId = birthday.Id,
                IsCustomizable = false,
                IsActive = true
            };
            var kinderjoyFavourFlower = new Hamper
            {
                Name = "Kinderjoy Favour Flower",
                Slug = "kinderjoy-favour-flower",
                Description = "Kinderjoy eggs arranged into a flower-bouquet-style favour.",
                BasePrice = 39m,
                CategoryId = birthday.Id,
                IsCustomizable = false,
                IsActive = true
            };
            var partyPopperCones = new Hamper
            {
                Name = "Party Popper Cones",
                Slug = "party-popper-cones",
                Description = "Colorful cone-shaped candy party poppers for return gifts.",
                BasePrice = 22m,
                CategoryId = birthday.Id,
                IsCustomizable = false,
                IsActive = true
            };

            // ---- Hampers: Festival & Custom placeholders ----
            var diwaliDeluxeBox = new Hamper
            {
                Name = "Diwali Deluxe Box",
                Slug = "diwali-deluxe-box",
                Description = "A festive box with dry fruits, diyas, and sweets for Diwali.",
                BasePrice = 899m,
                CategoryId = festival.Id,
                IsCustomizable = true,
                IsActive = true
            };
            var buildYourOwnHamper = new Hamper
            {
                Name = "Build Your Own Hamper",
                Slug = "build-your-own-hamper",
                Description = "Fully customizable hamper where you choose every item inside.",
                BasePrice = 0m,
                CategoryId = custom.Id,
                IsCustomizable = true,
                IsActive = true
            };

            // ---- Seed Hamper Items (Included inside the hampers) ----
            tumblerHamper.Items = new List<HamperItem>
            {
                new HamperItem { Name = "Personalized Coffee Tumbler (Duo)", Price = 0m },
                new HamperItem { Name = "Handcrafted Designer Rakhi Thread", Price = 0m },
                new HamperItem { Name = "Mini Glass Jar with Roli & Chawal", Price = 0m },
                new HamperItem { Name = "Premium Salted Cashews (100g)", Price = 0m }
            };

            microHamper.Items = new List<HamperItem>
            {
                new HamperItem { Name = "Cadbury Dairy Milk Silk (60g)", Price = 0m },
                new HamperItem { Name = "Classic Rakhi Thread", Price = 0m },
                new HamperItem { Name = "Roli Chawal Tilak Packet", Price = 0m }
            };

            chocolateBouquet.Items = new List<HamperItem>
            {
                new HamperItem { Name = "Ferrero Rocher Chocolates (16 Pcs)", Price = 0m },
                new HamperItem { Name = "Cadbury Dairy Milk Bars (5 Pcs)", Price = 0m },
                new HamperItem { Name = "Red & Golden Crepe Paper Wrapping", Price = 0m },
                new HamperItem { Name = "Elegant Silk Ribbon Bow", Price = 0m }
            };

            kinderjoyBouquet.Items = new List<HamperItem>
            {
                new HamperItem { Name = "Kinder Joy Chocolate Eggs (8 Pcs)", Price = 0m },
                new HamperItem { Name = "Cartoon Character Kids Rakhi", Price = 0m },
                new HamperItem { Name = "Star-shaped Balloon Topper", Price = 0m }
            };

            lollypopHamper.Items = new List<HamperItem>
            {
                new HamperItem { Name = "Gourmet Fruit-Flavored Lollipops (10 Pcs)", Price = 0m },
                new HamperItem { Name = "Colorful Bow Tied Ribbon Wrap", Price = 0m }
            };

            miniHamper.Items = new List<HamperItem>
            {
                new HamperItem { Name = "Pringles Potato Chips (Original, 110g)", Price = 0m },
                new HamperItem { Name = "Cadbury Dairy Milk Silk (60g)", Price = 0m },
                new HamperItem { Name = "Handcrafted Beads Rakhi Thread", Price = 0m },
                new HamperItem { Name = "Roli Chawal Tilak Pack", Price = 0m }
            };

            smileHamper.Items = new List<HamperItem>
            {
                new HamperItem { Name = "Personalized Small Photo Cards (5 Pcs)", Price = 0m },
                new HamperItem { Name = "Sunflower Themed Foil Balloon", Price = 0m },
                new HamperItem { Name = "Mini Chocolate Gift Box", Price = 0m },
                new HamperItem { Name = "Oatmeal Chocolate Chip Cookies", Price = 0m }
            };

            shinchanHamper.Items = new List<HamperItem>
            {
                new HamperItem { Name = "Shin-chan Cute Plush Toy (Small)", Price = 0m },
                new HamperItem { Name = "Kellogg's Chocos Box (100g)", Price = 0m },
                new HamperItem { Name = "Fruity Jelly Lollipops (5 Pcs)", Price = 0m },
                new HamperItem { Name = "Kids Shin-chan Character Rakhi", Price = 0m }
            };

            // Seeding items for Birthday hampers
            animalPopperFavour.Items = new List<HamperItem>
            {
                new HamperItem { Name = "Cute Animal Candy Popper Toy", Price = 0m },
                new HamperItem { Name = "Mixed Fruit Chews Candy Pack", Price = 0m }
            };

            catFaceLollipopFavour.Items = new List<HamperItem>
            {
                new HamperItem { Name = "Gourmet Cat-Face Sugar Lollipop", Price = 0m },
                new HamperItem { Name = "Personalized Happy Birthday Favor Tag", Price = 0m }
            };

            teddyLollipopBag.Items = new List<HamperItem>
            {
                new HamperItem { Name = "Teddy-shaped Craft Paper Gift Bag", Price = 0m },
                new HamperItem { Name = "Strawberry Heart Lollipop (Large)", Price = 0m }
            };

            heartChocolateWreath.Items = new List<HamperItem>
            {
                new HamperItem { Name = "Heart-shaped Milk Chocolate Wreath (150g)", Price = 0m },
                new HamperItem { Name = "Glitter Red Ribbon Wrap", Price = 0m }
            };

            snailLollipopFavour.Items = new List<HamperItem>
            {
                new HamperItem { Name = "Colorful Snail-shaped Lollipop Topper", Price = 0m },
                new HamperItem { Name = "Mini Fruity Lollipops (2 Pcs)", Price = 0m }
            };

            starChocolateFavour.Items = new List<HamperItem>
            {
                new HamperItem { Name = "Golden Foil-wrapped Star Chocolates (6 Pcs)", Price = 0m },
                new HamperItem { Name = "Metallic Gold Favor Pouch", Price = 0m }
            };

            kinderjoyFavourFlower.Items = new List<HamperItem>
            {
                new HamperItem { Name = "Kinder Joy Flower-style Bouquet (3 Eggs)", Price = 0m },
                new HamperItem { Name = "Mini Happy Birthday Greeting Card", Price = 0m }
            };

            partyPopperCones.Items = new List<HamperItem>
            {
                new HamperItem { Name = "Cone-shaped Candy Party Popper Toy", Price = 0m },
                new HamperItem { Name = "Mixed Fruit Jellies (3 Pcs)", Price = 0m }
            };

            // Seeding items for Festival / Custom hampers
            diwaliDeluxeBox.Items = new List<HamperItem>
            {
                new HamperItem { Name = "Handcrafted Brass Diyas (2 Pcs)", Price = 0m },
                new HamperItem { Name = "Premium Mixed Dry Fruits (Cashews, Almonds, Pistachios - 200g)", Price = 0m },
                new HamperItem { Name = "Traditional Kaju Katli Sweet Box (150g)", Price = 0m },
                new HamperItem { Name = "Scented Rose Tealight Candles (4 Pcs)", Price = 0m }
            };

            buildYourOwnHamper.Items = new List<HamperItem>
            {
                new HamperItem { Name = "Gourmet Chocolates Selection", Price = 150m },
                new HamperItem { Name = "Personalized Ceramic Mug", Price = 250m },
                new HamperItem { Name = "Scented Soy Wax Candle", Price = 200m },
                new HamperItem { Name = "Premium Roasted Almonds (150g)", Price = 180m },
                new HamperItem { Name = "Handwritten Elegant Greeting Card", Price = 50m }
            };

            var allHampers = new List<Hamper>

            {
                tumblerHamper, microHamper, chocolateBouquet, kinderjoyBouquet, lollypopHamper, miniHamper, smileHamper, shinchanHamper,
                animalPopperFavour, catFaceLollipopFavour, teddyLollipopBag, heartChocolateWreath, snailLollipopFavour, starChocolateFavour, kinderjoyFavourFlower, partyPopperCones,
                diwaliDeluxeBox, buildYourOwnHamper
            };
            context.Hampers.AddRange(allHampers);
            context.SaveChanges();

            // ---- Hamper Images (placeholder paths - replace with actual uploaded image URLs) ----
            var images = new List<HamperImage>
            {
                new HamperImage { HamperId = tumblerHamper.Id, ImageUrl = "/images/hampers/rakhi/Tumbler Hamper.png", IsPrimary = true, SortOrder = 1 },
                new HamperImage { HamperId = microHamper.Id, ImageUrl = "/images/hampers/rakhi/Micro Hamper.png", IsPrimary = true, SortOrder = 1 },
                new HamperImage { HamperId = chocolateBouquet.Id, ImageUrl = "/images/hampers/rakhi/Chocolate Bouquet.png", IsPrimary = true, SortOrder = 1 },
                new HamperImage { HamperId = kinderjoyBouquet.Id, ImageUrl = "/images/hampers/rakhi/Kinderjoy Bouquet.png", IsPrimary = true, SortOrder = 1 },
                new HamperImage { HamperId = lollypopHamper.Id, ImageUrl = "/images/hampers/rakhi/Lollipopp Hamper.png", IsPrimary = true, SortOrder = 1 },
                new HamperImage { HamperId = miniHamper.Id, ImageUrl = "/images/hampers/rakhi/Mini Hamper.png", IsPrimary = true, SortOrder = 1 },
                new HamperImage { HamperId = smileHamper.Id, ImageUrl = "/images/hampers/rakhi/Smile Hamper.png", IsPrimary = true, SortOrder = 1 },
                new HamperImage { HamperId = shinchanHamper.Id, ImageUrl = "/images/hampers/rakhi/Shinchan Hamper.png", IsPrimary = true, SortOrder = 1 },

                new HamperImage { HamperId = animalPopperFavour.Id, ImageUrl = "/images/hampers/birthday/Animal Popper Favour.png", IsPrimary = true, SortOrder = 1 },
                new HamperImage { HamperId = catFaceLollipopFavour.Id, ImageUrl = "/images/hampers/birthday/Cat Face Lollipop Favour.png", IsPrimary = true, SortOrder = 1 },
                new HamperImage { HamperId = teddyLollipopBag.Id, ImageUrl = "/images/hampers/birthday/Teddy Lollipop Bag.png", IsPrimary = true, SortOrder = 1 },
                new HamperImage { HamperId = heartChocolateWreath.Id, ImageUrl = "/images/hampers/birthday/Heart Chocolate Wreath.png", IsPrimary = true, SortOrder = 1 },
                new HamperImage { HamperId = snailLollipopFavour.Id, ImageUrl = "/images/hampers/birthday/Snail Lollipop Favour.png", IsPrimary = true, SortOrder = 1 },
                new HamperImage { HamperId = starChocolateFavour.Id, ImageUrl = "/images/hampers/birthday/Star Chocolate Favour.png", IsPrimary = true, SortOrder = 1 },
                new HamperImage { HamperId = kinderjoyFavourFlower.Id, ImageUrl = "/images/hampers/birthday/Kinderjoy Favour Flower.png", IsPrimary = true, SortOrder = 1 },
                new HamperImage { HamperId = partyPopperCones.Id, ImageUrl = "/images/hampers/birthday/Party Popper Cones.png", IsPrimary = true, SortOrder = 1 },

                new HamperImage { HamperId = diwaliDeluxeBox.Id, ImageUrl = "/images/hampers/festival/Diwali Deluxe Box.png", IsPrimary = true, SortOrder = 1 },
                new HamperImage { HamperId = buildYourOwnHamper.Id, ImageUrl = "/images/hampers/custom/Build Your Own Hamper (Custom).png", IsPrimary = true, SortOrder = 1 },
            };
            context.HamperImages.AddRange(images);

            // ---- City Availability: all hampers available in all 3 cities for now ----
            foreach (var hamper in allHampers)
            {
                hamper.AvailableCities.Add(jaipur);
                hamper.AvailableCities.Add(pune);
                hamper.AvailableCities.Add(kota);
            }

            context.SaveChanges();
        }
    }
}
