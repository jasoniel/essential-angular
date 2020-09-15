import { Product } from "./product.model";

export class Rating {
  constructor(
    public ratingId?: number,
    public stars?: string,
    public product?: Product
  ) {}
}
